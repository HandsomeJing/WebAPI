module.exports = {
  transpileDependencies: [],
  lintOnSave: false, // 禁用 ESLint 检查以避免构建问题
  devServer: {
    port: 8080,
    proxy: {
      '/api': {
        target: 'http://localhost:7032',
        changeOrigin: true,
  secure: false,
  logLevel: 'debug',
  pathRewrite: { '^/api': '' }
      }
    }
  }
}
