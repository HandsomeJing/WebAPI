import axios from "./AxiosHelper"

import { IProductInputDto, IProductPropInputDto, IShoppingCartInputDto } from '@/interfaces/ProductList'

export const getProduct = async (data: IProductInputDto) => {
   // 过滤空字符串/空值，防止后端收到 productProps="" 导致 Split 错误
   const params: any = { belongTypeName: data.belongTypeName }
   if (data.searchText && data.searchText.trim() !== "") params.searchText = data.searchText
   if (data.productType && data.productType.trim() !== "") params.productType = data.productType
   if (data.productProps && data.productProps.trim() !== "") params.productProps = data.productProps
   if (data.sort && data.sort.trim() !== "") params.sort = data.sort
   if (data.pageIndex) params.pageIndex = data.pageIndex
   var res = await axios.get("/api/Product", { params })
   return res.data;
}
export const getBelogType = async () => {
   var res = await axios.get("/api/Product/BlongType")
   return res.data;
}
export const getType = async (belongTypeName: string) => {
   var res = await axios.get("/api/Product/type?belongTypeName=" + belongTypeName)
   return res.data;
}

export const getProp = async (data: IProductPropInputDto) => {
   var res = await axios.get("/api/Product/props", { params: data })
   return res.data;
}

export const addCart = async(data: IShoppingCartInputDto) =>{
   var res = await axios.post("/api/ShoppingCart", data);
   return res.data;
}

