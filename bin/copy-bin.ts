import fs from 'fs';
import path from 'path';

fs.cpSync(
  path.join(__dirname, 'umodel'),
  path.normalize(path.join(__dirname, '..', 'dist', 'bin')),
  { dereference: true, recursive: true});
