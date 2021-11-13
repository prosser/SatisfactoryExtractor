import { execSync } from 'child_process';
import { copyFileSync } from 'fs';
import path from 'path';

const packDir = path.resolve(__dirname, '../dist');
copyFileSync(path.resolve(__dirname, '../package.json'), path.join(packDir, 'package.json'));
execSync('npm pack', { cwd: packDir });
