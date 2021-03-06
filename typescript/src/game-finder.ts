import * as vdf from '@node-steam/vdf';
import fs from 'fs';
import path from 'path';
import { enumerateValues, HKEY, RegistryValueType } from 'registry-js';
import { Channel, GameFinderOptions } from './game-finder-options';

export class GameFinder {
  public static findGame(options: GameFinderOptions): string {
    try {
      if (!options.onlyEpic) {
        try {
          return GameFinder.getSteamGamePath(options.channel);
        } catch (err) {
          if (options.onlySteam) throw err;
        }
      }
      if (!options.onlySteam)
        return GameFinder.getEpicGamesPath(options.channel);
    }
    catch (err) {
      throw new Error(`Could not find ${options.channel} installation: ${err.message ?? 'unknown error'}. Options=${JSON.stringify(options)}`);
    }
    throw new Error('An unknown error ocurred.');
  }

  private static getRegistryString = (path: string, valueName: string): string | undefined => {
    const ucName = valueName.toLocaleUpperCase();
    for (const value of enumerateValues(HKEY.HKEY_LOCAL_MACHINE, path)) {
      if (value.name.toLocaleUpperCase() === ucName && value.type === RegistryValueType.REG_SZ)
        return value.data;
    }
    return undefined;
  };

  private static findWindowsSteamLibrariesVdf(): string {
    const steamPath = GameFinder.getRegistryString('SOFTWARE\\WOW6432Node\\Valve\\Steam', 'InstallPath');
    if (!steamPath) throw new Error('Steam is not installed properly.');

    // Steam can have multiple library locations, which are in the VDF format in a known location.
    const librariesVdf = path.resolve(steamPath, 'steamapps', 'libraryfolders.vdf');

    return librariesVdf;
  }

  private static findWindowsEpicManifests(): string {
    const appDataPath = GameFinder.getRegistryString('SOFTWARE\\WOW6432Node\\Epic Games\\EpicGamesLauncher', 'AppDataPath');
    if (!appDataPath) {
      throw new Error('Epic Games registry entry not found');
    }

    return path.resolve(appDataPath, 'Manifests');
  }

  private static getEpicGamesPath(channel: Channel): string {
    // This is the Epic Games catalog item id for the Experimental fork of Satisfactory.
    // We could use DisplayName, but that would be less accurate.
    const catalogIds: Record<Channel, string> = {
      experimental: 'ef4a63daa7d4420e91420a72050be89d',
      earlyaccess: 'b915dfe8dcf74770841c82a4162dc954'
    };

    const catalogId = catalogIds[channel];

    // in the Manifests folder, there will be one .item file per installed game.
    const manifestsFolder = this.findWindowsEpicManifests();

    if (!fs.existsSync(manifestsFolder)) {
      throw new Error(`Epic Games manifests directory '${manifestsFolder}' does not exist`);
    }

    for (const itemFileName of fs.readdirSync(manifestsFolder)) {
      if (!itemFileName.endsWith('.item')) continue;
      const itemPath = path.resolve(manifestsFolder, itemFileName);

      // Epic's manifests are in JSON format. Games are keyed by the catalog item id.
      const manifest = JSON.parse(fs.readFileSync(itemPath, { encoding: 'utf8' }));
      if (manifest?.CatalogItemId === catalogId)
        return manifest.InstallLocation;
    }
    throw new Error('Could not find installed game in any Epic Games library');
  }

  private static getSteamGamePath(channel: Channel): string {
    // look in the Windows registry to find Steam's installation path
    const steamAppId = '526870';

    const librariesVdf = this.findWindowsSteamLibrariesVdf();

    if (!fs.existsSync(librariesVdf)) throw new Error(`${librariesVdf} does not exist.`);

    const manifest = vdf.parse(fs.readFileSync(librariesVdf, { encoding: 'utf8' }));
    if (!manifest?.libraryfolders) throw new Error(`No libraryfolders in Steam VDF at ${librariesVdf}`);

    // find the library that contains the Satisfactory game
    for (const k in manifest.libraryfolders) {
      if (!Object.prototype.hasOwnProperty.call(manifest.libraryfolders, k) || typeof manifest.libraryfolders[k] !== 'object')
        continue;

      const library = manifest.libraryfolders[k];

      // sanity checking...
      if (!library || typeof library !== 'object' ||
        !library.path || !fs.existsSync(library.path) ||
        !library.apps || typeof library.apps !== 'object' ||
        // if the library contains the game, this will be present.
        library.apps[steamAppId] === undefined) {
        continue;
      }

      const steamappsDir = path.resolve(library.path, 'steamapps');

      // Steam thinks the game is installed, so look for the game's local install path in its VDF manifest.
      const appManifestPath = path.resolve(steamappsDir, `appmanifest_${steamAppId}.acf`);
      if (!fs.existsSync(appManifestPath)) {
        continue;
      }

      // the manifest exists, so parse it
      const appManifest = vdf.parse(fs.readFileSync(appManifestPath, { encoding: 'utf8' })) as SteamAppManifest;

      // verify that this is the correct channel
      if (getSteamChannel(appManifest) !== channel) {
        continue;
      }

      const installDir = appManifest?.AppState?.installdir;
      if (!installDir) {
        continue;
      }

      // Steam thinks the game is installed in this library, but sanity check that it actually still exists.
      const installPath = path.resolve(steamappsDir, 'common', installDir);
      if (!fs.existsSync(installPath)) continue;

      return installPath;
    }

    throw new Error('Could not find installed game in any Steam library');
  }
}

interface SteamAppManifest {
  AppState: {
    appid: string;
    installdir?: string;
    UserConfig?: {
      betakey?: string
    }
  };
}

const getSteamChannel = (appManifest: SteamAppManifest): Channel => {
  return appManifest.AppState.UserConfig?.betakey === 'experimental'
    ? 'experimental'
    : 'earlyaccess';
};