import axios from "./AxiosHelper"


export const getCarts = async () => {
    var res = await axios.get("/api/ShoppingCart");
    return res.data;
}

export const updateCartSelect = async (cartGuids: string[], cartSelected: boolean, productNum: number) => {
    console.log(productNum)
    var res = await axios.post("/api/ShoppingCart/CartSelected", { cartGuids, cartSelected, productNum });
    return res.data;
}
