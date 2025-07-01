import { fileURLToPath, URL } from 'node:url';
import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-vue';
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';
import { env } from 'process';

const isLocal = process.env.NODE_ENV !== 'production';

let httpsOptions = undefined;
let target = 'https://localhost:7153';

const devPort = 55281;
const prodPort = parseInt(env.PORT || '10000');
const port = isLocal ? devPort : prodPort;

if (isLocal) {
  const baseFolder =
    env.APPDATA !== undefined && env.APPDATA !== ''
      ? `${env.APPDATA}/ASP.NET/https`
      : `${env.HOME}/.aspnet/https`;

  const certificateName = "kaizen.client";
  const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
  const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

  if (!fs.existsSync(baseFolder)) {
    fs.mkdirSync(baseFolder, { recursive: true });
  }

  if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
    const result = child_process.spawnSync('dotnet', [
      'dev-certs',
      'https',
      '--export-path',
      certFilePath,
      '--format',
      'Pem',
      '--no-password',
    ], { stdio: 'inherit' });

    if (result.status !== 0) {
      console.warn('⚠️ Could not create dev certificate. HTTPS will be disabled.');
    }
  }

  if (fs.existsSync(certFilePath) && fs.existsSync(keyFilePath)) {
    httpsOptions = {
      key: fs.readFileSync(keyFilePath),
      cert: fs.readFileSync(certFilePath),
    };
  }

  target = env.ASPNETCORE_HTTPS_PORT
    ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
    : env.ASPNETCORE_URLS
      ? env.ASPNETCORE_URLS.split(';')[0]
      : 'https://localhost:7153';
}

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [plugin()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  server: {
    proxy: {
      '/api': {
        target,
        secure: false,
      },
    },
    port,
    https: httpsOptions,
  },
  preview: {
    port,
    host: true,
    https: false,
  }
});
