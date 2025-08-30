import axios from "./AxiosHelper"

export const getCartNum = async () => {
   var res = await axios.get("/api/ShoppingCart/num");
   return res.data;
}

