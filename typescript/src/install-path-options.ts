export interface InstallPathOptions {
  installPath: string
}

export const isInstallPathOptions = (options: unknown): options is InstallPathOptions => {
  return (options as InstallPathOptions)?.installPath !== undefined;
};
