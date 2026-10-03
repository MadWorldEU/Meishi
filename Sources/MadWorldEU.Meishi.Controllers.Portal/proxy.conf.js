// Aspire injects the API endpoint as API_HTTPS / API_HTTP when the Portal runs through the AppHost.
const target = process.env.API_HTTPS || process.env.API_HTTP;

module.exports = target
  ? {
      '/api': {
        target,
        secure: false,
        changeOrigin: true,
        pathRewrite: { '^/api': '' },
      },
    }
  : {};
