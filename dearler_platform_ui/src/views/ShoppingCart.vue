<template>
  <div>
    <div class="cart-list">
      <ul>
    <li v-for="type in types" :key="type.typeNo">
          <p>
            <i
              :class="{ 'cart-select': type.typeSelected }"
              @click="onSelecType(type)"
              >✔</i
            >
            <span>{{ transTypeWhenNull(type.typeName) }}</span>
          </p>
          <template v-for="cart in carts">
            <div v-if="cart.productDto?.typeNo == type.typeNo" :key="cart.id">
              <i
                :class="{ 'cart-select': cart.cartSelected }"
                @click="onSelectCart(cart)"
                >✔</i
              >
              <img :src="cart.productDto?.productPhoto?.productPhotoUrl || '/img/dealerImgs/picUser.png'" alt="商品图片" />
              <p class="p-name">{{ cart.productDto?.productName }}</p>
              <p class="p-price">
                &yen;{{
                  transPrice(
                    transPriceWhenNull(cart.productDto?.productSale?.salePrice)
                  )
                }}
              </p>
              <p class="p-num">
                <span class="sub-num" @click="onSubNum(cart)">-</span>
                <input
                  type="text"
                  name=""
                  id=""
                  :value="cart.productNum"
                  @change="onChangeNum(cart)"
                />
                <span class="add0num" @click="onAddNum(cart)">+</span>
                <b>块</b>
              </p>
            </div>
          </template>
        </li>
        <!-- <li>
          <p>
            <i class="cart-select">✔</i>
            <span>处理产品</span>
          </p>
          <div>
            <i class="cart-select">✔</i>
            <img src="" alt="" />
            <p class="p-name">0.9密度板</p>
            <p class="p-price">&yen;100.00</p>
            <p class="p-num">
              <span class="sub-num" @click="subNum()">-</span>
              <input
                type="text"
                name=""
                id=""
                :value="buyNum"
                @change="changeNum()"
              />
              <span class="add0num" @click="addNum()">+</span>
              <b>块</b>
            </p>
          </div>
        </li>
        <li>
          <p>
            <i></i>
            <span>处理产品</span>
          </p>
          <div>
            <i></i>
            <img src="" alt="" />
            <p class="p-name">0.9密度板</p>
            <p class="p-price">&yen;100.00</p>
            <p class="p-num">
              <span class="sub-num" @click="subNum()">-</span>
              <input
                type="text"
                name=""
                id=""
                :value="buyNum"
                @change="changeNum()"
              />
              <span class="add0num" @click="addNum()">+</span>
              <b>块</b>
            </p>
          </div>
          <div>
            <i></i>
            <img src="" alt="" />
            <p class="p-name">0.9密度板</p>
            <p class="p-price">&yen;100.00</p>
            <p class="p-num">
              <span class="sub-num" @click="subNum()">-</span>
              <input
                type="text"
                name=""
                id=""
                :value="buyNum"
                @change="changeNum()"
              />
              <span class="add0num" @click="addNum()">+</span>
              <b>块</b>
            </p>
          </div>
        </li> -->
      </ul>
    </div>
    <div class="total-pad">
      <i :class="{ 'cart-select': isAllSelected }" @click="toggleAll">✔</i>
      <span>全选</span>
  <span>
        合计：&yen; <b>{{ transPrice(totalPrice) }}</b>
      </span>
      <button
        class="order-btn"
        :class="{ 'order-btn--enabled': hasSelected }"
        :disabled="!hasSelected"
        @click="goOrderConfirm"
      >确定下单</button>
    </div>
  </div>
</template>

<script>
import { defineComponent, onMounted, reactive, ref, toRefs, watch, computed } from 'vue'
import { transPrice } from '../utility/common'
import { shoppingCartNum } from '../store'
import { getCarts, updateCartSelect } from '../HttpRequests/ShoppingCartRequest'
import { useStore } from 'vuex'

export default defineComponent({
  name: 'ShoppingCart',
  //   mounted () {
  //     this.$store.comm
  //     this.$store.dispatch('setFootMenuIndexAsync', 2)
  //   methods: {

  //   }
  setup () {
    // 数据发生改变时，这个方式不能及时获取变化后的数据
    // const productNumRef = ref();
    const store = useStore()
    const shoppingCartInfo = reactive({
      carts: [],
      types: [],
      isAllSelected: false,
      totalPrice: 0,
      onAddNum (cart) {
        cart.productNum++
        updateCartSelect([cart.cartGuid], cart.cartSelected, cart.productNum)
      },
      onSubNum: async cart => {
        if (cart.productNum > 0) {
          cart.productNum--

          var res = await updateCartSelect(
            [cart.cartGuid],
            cart.cartSelected,
            cart.productNum
          )
          if (res == 'Remove') {
            var index = shoppingCartInfo.carts.findIndex(
              m => m.cartNo == cart.cartNo
            )
            shoppingCartInfo.carts.splice(index, 1)
          }
        }
      },
      onChangeNum (cart) {
        // 通过点击事件，获取当前控件的值
        var currNum = event.target.value
        // 判断这个值是个数字并且大于0时，把这个值赋值给响应式变量cart.productNum
        // 否则用cart.productNum的值还原input控件中的内容
        if (!isNaN(currNum) && currNum > 0) {
          cart.productNum = currNum
        } else {
          event.target.value = cart.productNum
        }
      },

      /**
       * 获取购物车信息
       */
      onGetShoppingCarts: async () => {
        const res = await getCarts()
        shoppingCartInfo.carts = (res && res.carts) ? res.carts : []
        // 兼容后端返回的大小写（TypeNo/TypeName 或 typeNo/typeName）
        let types = (res && res.types) ? res.types : []
        types = (types || []).map(function (t) {
          return {
            typeNo: t.typeNo ?? t.TypeNo ?? t.typeno,
            typeName: t.typeName ?? t.TypeName ?? t.typename,
            typeSelected: !!(t.typeSelected ?? t.TypeSelected)
          }
        })
        // 若后端未返回 types，则根据 carts 推导
        if ((!types || types.length === 0) && shoppingCartInfo.carts && shoppingCartInfo.carts.length) {
          var map = new Map()
          shoppingCartInfo.carts.forEach(function (c) {
            var no = c.productDto && c.productDto.typeNo
            var name = c.productDto && c.productDto.typeName
            if (no && !map.has(no)) map.set(no, name)
          })
          types = Array.from(map.entries()).map(function (pair) {
            return { typeNo: pair[0], typeName: pair[1], typeSelected: false }
          })
        }
        shoppingCartInfo.types = types
      },

      /**
       * 点击选择购物车时触发
       */
      onSelectCart: cart => {
        cart.cartSelected = !cart.cartSelected
        updateCartSelect([cart.cartGuid], cart.cartSelected, cart.productNum)
        // 找到类型中对应设置物品的类型
        // var type = shoppingCartInfo.types.filter(
        //   m => m.typeNo == cart.productDto?.typeNo
        // )[0]
        // var cartsOfType = shoppingCartInfo.carts.filter(
        //   m => m.productDto?.typeNo == type?.typeNo
        // )
        // shoppingCartInfo.checkTypeSelected();
      },

      /**
       * 选择类型时触发
       */
      onSelecType: type => {
        var cartGuids = []
        type.typeSelected = !type.typeSelected
        shoppingCartInfo.carts
          .filter(m => m.productDto?.typeNo == type?.typeNo)
          .forEach(m => {
            cartGuids.push(m.cartGuid)
            m.cartSelected = type?.typeSelected
          })
        updateCartSelect(cartGuids, type.typeSelected)
        // shoppingCartInfo.checkAllSelected()
      },

      /**
       * 检测是否选中某一类下的所有物品
       */
      checkTypeSelected: () => {
        shoppingCartInfo.types.forEach(type => {
          var cartsOfType = shoppingCartInfo.carts.filter(
            m => m.productDto?.typeNo == type?.typeNo
          )
          // 查看当前类型下的物品是否都被选择，是则将属于类型选中
          // 否则不选中
          if (cartsOfType.every(m => m.cartSelected)) {
            type.typeSelected = true
          } else {
            type.typeSelected = false
          }
        })
        shoppingCartInfo.checkAllSelected()
      },

      /**
       * 检测是否选中所有物品
       */
      checkAllSelected: carts => {
        if (shoppingCartInfo.carts.every(m => m.cartSelected)) {
          shoppingCartInfo.isAllSelected = true
        } else {
          shoppingCartInfo.isAllSelected = false
        }
      },
        toggleAll: () => {
          const next = !shoppingCartInfo.isAllSelected
          shoppingCartInfo.isAllSelected = next
          const cartGuids = shoppingCartInfo.carts.map(c => c.cartGuid)
          shoppingCartInfo.carts.forEach(c => (c.cartSelected = next))
          updateCartSelect(cartGuids, next)
        },
      /**
       * 计算总价
       */
      calcTotalPrice: carts => {
        shoppingCartInfo.totalPrice = 0
        let currentCartNum = 0
        carts[0]
          .filter(c => c.cartSelected == true)
          .forEach(c => {
            var singlePrice = c.productDto?.productSale?.salePrice ?? 0
            shoppingCartInfo.totalPrice += singlePrice * c.productNum
            currentCartNum += c.productNum
          })
        // shoppingCartNum.value = currentCartNum;
        store.dispatch('shoppingCart/setCartNum', currentCartNum)
      },
      transTypeWhenNull: type => type ?? '未分类产品',
      transPriceWhenNull: price => price ?? 0
    })

    /**
     * 对carts进行深度监听
     */
    watch(
      [() => shoppingCartInfo.carts],
      (newValue, oldValue) => {
        shoppingCartInfo.checkTypeSelected()
        shoppingCartInfo.calcTotalPrice(newValue)
      },
      {
        deep: true // 是否深度监听，一般监听对象或者是数组，咱都需要深度监听
      }
    )

    /**
     * 页面加载时触发
     */
    onMounted(async () => {
      await shoppingCartInfo.onGetShoppingCarts()
      shoppingCartInfo.checkTypeSelected()
    })

      const hasSelected = computed(() => {
        if (!shoppingCartInfo.carts?.length) return false
        return shoppingCartInfo.carts.some(c => c.cartSelected && c.productNum > 0)
      })

      const goOrderConfirm = () => {
        if (!hasSelected.value) return
        window.location.hash = '#/layoutMain/orderConfirm'
      }
      return { ...toRefs(shoppingCartInfo), transPrice, goOrderConfirm, hasSelected }
  }
})
</script>

<style lang="scss" scoped>
.cart-list {
  text-align: left;
  ul {
    margin-bottom: 108px;

    li {
      background-color: #fff;
      margin-bottom: 12px;

      > p {
        padding-left: 46px;
        position: relative;
        height: 46px;
        border-bottom: 1px solid #ddd;

        i {
          border: 1px solid #a9a9a9;
          width: 18px;
          height: 18px;
          line-height: 18px;
          border-radius: 18px;
          position: absolute;
          left: 13px;
          top: 13px;
          text-align: center;
          font-size: 12px;
          color: #fff;
          font-style: normal;
        }

        i.cart-select {
          background-color: crimson;
          border: 1px solid crimson;
        }

        span {
          display: inline-block;
          border-left: 3px solid crimson;
          height: 28px;
          margin: 9px 0;
          padding-left: 8px;
          line-height: 30px;
        }
      }

      div {
        padding-left: 46px;
        position: relative;
        height: 98px;
        padding: 8px 14px 8px 148px;

        i {
          border: 1px solid #a9a9a9;
          width: 18px;
          height: 18px;
          line-height: 18px;
          border-radius: 18px;
          position: absolute;
          left: 13px;
          top: 28px;
          text-align: center;
          font-size: 12px;
          color: #fff;
          font-style: normal;
        }

        i.cart-select {
          background-color: crimson;
          border: 1px solid crimson;
        }

        img {
          width: 68px;
          height: 68px;
          background-color: #ccc;
          position: absolute;
          left: 58px;
          top: 20px;
        }

        p.p-name {
          font-size: 13px;
          margin-top: 10px;
          height: 30px;
        }
        p.p-price {
          font-size: 13px;
          height: 20px;
          color: crimson;
        }
        p.p-num {
          text-align: right;
          padding-right: 20px;

          span {
            display: inline-block;
            width: 18px;
            height: 18px;
            border: 1px solid crimson;
            color: crimson;
            border-radius: 9px;
            text-align: center;
            line-height: 18px;
          }

          input {
            width: 28px;
            border: none 0px;
            outline: none;
            text-align: center;
          }

          b {
            font-weight: normal;
            margin-left: 10px;
            font-size: 13px;
          }
        }
      }
    }
  }
}

.total-pad {
  height: 58px;
  width: 100%;
  background-color: #383838;
  position: fixed;
  left: 0;
  bottom: 40px;

  i {
    display: inline-block;
    border: 1px solid #a9a9a9;
    width: 18px;
    height: 18px;
    line-height: 18px;
    border-radius: 18px;
    background-color: #fff;
    margin-left: 13px;
    margin-top: 20px;
    vertical-align: bottom;
    height: 18px;
    text-align: center;
    font-size: 12px;
    color: #fff;
    font-style: normal;
  }

  i.cart-select {
    background-color: crimson;
    border: 1px solid crimson;
  }

  span {
    color: #fff;
    margin-left: 6px;
    font-size: 13px;

    b {
      font-size: 15px;
    }
  }

  button {
    float: right;
    height: 58px;
    width: 120px;
    border: 0 none;
    background-color: #ddd;
    color: #aaa;
    font-size: 15px;
    font-weight: bold;
  }

  .order-btn--enabled {
    background-color: crimson;
    color: #fff;
    cursor: pointer;
  }
}
</style>
