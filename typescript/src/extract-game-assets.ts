import { execSync } from 'child_process';
import fs from 'fs';
import path from 'path';
import { GameFinderOptions, InstallPathOptions } from '..';
import { GameFinder } from './game-finder';
import { isGameFinderOptions } from './game-finder-options';
import { isInstallPathOptions } from './install-path-options';

const importPaks = (pakDir: string, targetDir: string): void => {
  // As of Update 5, 4.22 no longer works. Updated to the latest version that did work.
  const UNREAL_ENGINE_VERSION = 'ue4.27';

  if (!fs.existsSync(targetDir)) {
    fs.mkdirSync(targetDir, { recursive: true });
  }

  let variants = ['512', '256', '128', '64']
    .map(d => [`*_${d}.uasset`, `*_${d}_New.uasset`, `*_${d}_new.uasset`])
    .reduce((arr, cur) => {
      for (const n of cur) {
        arr.push(n);
      }
      return arr;
    }, []) as string[];


  // add variants that we don't need to permute
  variants = [...variants, '*Icon*.uasset'];

  const umodelExe = process.platform === 'linux' ? 'umodel' : 'umodel_64.exe';
  const commands = variants
    .map(x => `${umodelExe} -path="${pakDir}" -out="${targetDir}" -png -export ${x} -game=${UNREAL_ENGINE_VERSION}`);

  console.log(`Exporting PAK content from ${pakDir} using ${umodelExe}`);

  for (const command of commands) {
    console.log(command);
    try {
      execSync(command);
    }
    catch (err) {
      console.warn(`${command} failed with ${err}`);
    }
  }
};

export type ExtractGameAssetsOptions = (InstallPathOptions | GameFinderOptions) & {
  targetDir: string;
};

export const extractGameAssets = (options: ExtractGameAssetsOptions) => {
  const installPath = isInstallPathOptions(options)
    ? options.installPath
    : process.platform === 'linux' && !isInstallPathOptions(options)
      ? undefined
      : isGameFinderOptions(options)
        ? GameFinder.findGame(options)
        : undefined;
  if (!installPath) {
    // the GameFinder only works on Windows, because Satisfactory is only available for installation there.
    throw new Error('options must be InstallPathOptions when running on Linux.');
  }

  if (installPath === undefined) throw new Error('Unexpected error');

  const iconDir = path.resolve(options.targetDir, 'icons');
  const jsonDir = path.resolve(options.targetDir, 'json');
  try {
    fs.mkdirSync(iconDir, { recursive: true });
    fs.mkdirSync(jsonDir, { recursive: true });
  } catch {
    // ignore
  }

  const sourceDocsPath = path.resolve(installPath, 'CommunityResources', 'Docs', 'Docs.json');
  const targetDocsPath = path.resolve(jsonDir, 'Docs.json');

  // translate docs.json into UTF-8 and format it so it does not hobble IDEs
  const BOM = '\uFEFF';
  let docsJson = fs.readFileSync(sourceDocsPath, { encoding: 'utf16le'});
  if (docsJson.startsWith(BOM)) {
    docsJson = docsJson.substr(BOM.length);
  }

  const docs = JSON.parse(docsJson);
  console.log(`Writing Docs.json to '${targetDocsPath}'`);
  fs.writeFileSync(targetDocsPath, JSON.stringify(docs, null, '  '));

  const pakDir = path.resolve(installPath, 'FactoryGame', 'Content', 'Paks');

  importPaks(pakDir, iconDir);
};