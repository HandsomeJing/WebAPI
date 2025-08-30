import { createStore } from "vuex"

export default createStore({
    // state: {
    //     cartNum: 10
    // },
    // // 专门负责state数据的获取
    // getters: {
    //     getCartNum(state) {
    //         return state.cartNum;
    //     }
    // },
    // // mutations 必须同步执行，他负责操作state中数据的增删改
    // // store.commit("方法名", 参数)
    // // this.$stroe.commit() // used in vue2
    // mutations: {
    //     setCartNum(state, num) {
    //         state.cartNum = num;
    //     }
    // },
    // // 在mutations中执行异步操作，会引起数据失效，所以需要action在外面包一层，来专门进行异步操作
    // // 但是他最终调用的还是mutations
    // // store.dispatch("方法名", 参数)
    // // this.$stroe.dispatch() // used in vue2
    // actions: {
    //     setCartNum(context, num) {
    //         context.commit("setCartNum", num)
    //     }
    // },

    // 当项目非常庞大。需要管理的状态非常多的时候，Vuex允许我们将Store切分成多个模块（module）；每一个模块都拥有自己的state、mutation、getters、action
    modules: {
        shoppingCart: {
            namespaced:true,
            state: {
                cartNum: 10
            },
            // 专门负责state数据的获取
            getters: {
                getCartNum(state) {
                    return state.cartNum;
                }
            },
            // mutations 必须同步执行，他负责操作state中数据的增删改
            // store.commit("方法名", 参数)
            // this.$stroe.commit() // used in vue2
            mutations: {
                setCartNum(state, num) {
                    state.cartNum = num;
                }
            },
            // 在mutations中执行异步操作，会引起数据失效，所以需要action在外面包一层，来专门进行异步操作
            // 但是他最终调用的还是mutations
            // store.dispatch("方法名", 参数)
            // this.$stroe.dispatch() // used in vue2
            actions: {
                setCartNum(context, num) {
                    context.commit("setCartNum", num)
                }
            },
        }
    }
})