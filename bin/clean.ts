import fs from 'fs';
import path from 'path';
fs.rmSync(path.resolve(__dirname, '../dist'), { recursive: true, force: true });
