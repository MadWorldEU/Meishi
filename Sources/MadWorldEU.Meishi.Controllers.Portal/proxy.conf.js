// Aspire injects the API endpoint as API_HTTPS / API_HTTP when the Portal runs through the AppHost.
// Without the AppHost, fall back to the http port of the API's launch profiles (launchSettings.json).
const target = process.env.API_HTTPS || process.env.API_HTTP || 'http://localhost:5005';

module.exports = {
  '/api': {
    target,
    secure: false,
    changeOrigin: true,
    pathRewrite: { '^/api': '' },
  },
};
