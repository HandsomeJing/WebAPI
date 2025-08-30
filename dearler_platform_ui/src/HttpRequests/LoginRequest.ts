import axios from "./AxiosHelper"

export const login = async (data: any) => {
   // 后端 LoginController 的 [HttpPost] 默认路由为 /Login
   const res = await axios.post("/api/Login", data);
   return res.data;
}

