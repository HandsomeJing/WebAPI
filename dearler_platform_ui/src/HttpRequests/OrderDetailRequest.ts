import axios from "./AxiosHelper"

export const getOrderInfo = async (orderNo: string) => {
    var res = await axios.get("/api/OrderInfo", { params: { orderNo } })
    return res.data;
}

// 再次购买（后端会复制该订单为新订单）
export const buyAgain = async (saleOrderNo: string) => {
    const res = await axios.get("/api/OrderInfo/BuyAgain", { params: { saleOrderNo } })
    return res.data as boolean
}

// 取消订单
export const cancelOrder = async (saleOrderNo: string) => {
    const res = await axios.post("/api/OrderInfo/Cancel", null, { params: { saleOrderNo } })
    return res.data as boolean
}