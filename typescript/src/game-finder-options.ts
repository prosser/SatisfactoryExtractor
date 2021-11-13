import { FluentValidator } from './fluent-validator';

export type Channel = 'experimental' | 'earlyaccess';

export const isChannel = (obj: unknown): obj is Channel => {
  return obj === 'experimental' || obj === 'earlyaccess';
};

export interface GameFinderOptions {
  /** Satisfactory update channel */
  channel: Channel;

  /** true to only look for the game in Steam paths */
  onlySteam?: boolean;

  /** true to only look for the game in Epic Games paths */
  onlyEpic?: boolean;
}

export const isGameFinderOptions = (obj: unknown): obj is GameFinderOptions => {
  const options = obj as GameFinderOptions;
  return validate(options).errors.length == 0;
};

const validate = (options: GameFinderOptions) => {
  const validator = new FluentValidator()
    .mutuallyExclusive([
      { name: 'onlyEpic', test: () => !!options.onlyEpic && options.onlyEpic },
      { name: 'onlySteam', test: () => !!options.onlySteam && options.onlySteam }
    ])
    .oneOf('channel', options.channel, ['experimental', 'earlyaccess']);

  if (process.platform === 'linux') {
    validator.fail('Not supported on linux');
  }

  return validator;
};