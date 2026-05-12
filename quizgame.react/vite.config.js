import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
    plugins: [react()],

    resolve: {
        alias: {
            '@': '/src',
        },
    },

    server: {
        proxy: {

            '/api': {
                target: 'https://localhost:7001',
                changeOrigin: true,
                secure: false,
            },

            '/quizhub': {
                target: 'https://localhost:7001',
                changeOrigin: true,
                secure: false,
                ws: true
            }
        },
    },
})