import axios from "./AxiosHelper"

export const getInvoice = async () => {
   var res = await axios.get("/api/Customer/Invoice");
   return res.data;
}

export const getOrderConfirmCarts = async () => {
   var res = await axios.get("/api/OrderConfirm");
   return res.data;
}

export const addOrder=async (data:any)=>{
   var res = await axios.post("/api/OrderConfirm",data);
    return res.data;
}

// 获取当前客户最近订单号（用于下单成功后跳转详情）
export const getLatestOrderNo = async ()=>{
   const res = await axios.get("/api/OrderConfirm/LatestOrderNo");
   return res.data as string;
}