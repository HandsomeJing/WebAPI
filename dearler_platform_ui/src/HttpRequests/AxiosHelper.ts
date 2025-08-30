import axios from "axios"
import { useRoute, useRouter } from "vue-router";

// 使用相对根路径，配合 devServer 代理将 /api 重写到后端，避免跨域并兼容无 /api 前缀的后端路由
axios.defaults.baseURL = "/"

// axios拦截器分成两类，第一类叫做请求拦截器，第二类叫做响应拦截器
// 请求拦截器
axios.interceptors.request.use(
    config => {
        console.log('发送HTTP请求:', config.method?.toUpperCase(), config.url, config.data)
        // 判断是否存在token值，如果存在则在请求前给header中加上token
        if (localStorage["token"]) {
            if (config?.headers != null)
                (config.headers as any).Authorization = "Bearer " + localStorage["token"]; // 请求头加上token
        }
        return config;
    },
    error => {
        console.error('请求拦截器错误:', error)
        return Promise.reject(error);
    }
)
// 响应拦截器
axios.interceptors.response.use(
    response => {
        console.log('HTTP响应:', response.status, response.data)
        return response;
    },
    error => {
        console.error('HTTP请求错误:', error.response?.status, error.response?.data, error.message)
        if (error.response?.status === 401) {
            console.log('认证失败，清除token')
            localStorage.removeItem('token')
            localStorage.removeItem('cno')
            // 使用 Hash 路由，直接跳回登录页
            if (!location.hash.endsWith('/')) {
                location.hash = '#/'
            }
        }
        return Promise.reject(error);
    }
)
export default axios;
