<template>
  <div>
    <!-- 搜索面板 -->
    <div class="search-pad">
      <el-input
        v-model="searchText"
        placeholder="搜索商品"
        clearable
        size="large"
        @focus="searchFocus()"
        @blur="searchBlur()"
        @input="onSearchInput"
      />
  <el-button v-show="isShowSearchBtn" type="primary" size="small" @click="onSearchInput">搜索</el-button>
  <el-button v-show="!isShowSearchBtn" size="small" @click="showRight()">筛选</el-button>
    </div>
    <!-- 物品大类面板 -->
    <div class="system-pad">
      <div
        v-for="belogType in belogTypes"
        :key="belogType.belongTypeName"
        :class="[
          'system-item',
          { 'system-select': systemIndex == belogType.belongTypeName },
        ]"
        @click="selectSystemProduct(belogType.belongTypeName)"
      >
        <span>{{ belogType.belongTypeName }}</span>
      </div>
    </div>
    <!-- 物品展示列表 -->
    <div class="product-list">
      <el-skeleton :loading="loadingProducts" animated :count="3">
        <template #template>
          <div style="display:flex;gap:12px;padding:12px;">
            <el-skeleton-item variant="image" style="width:66px;height:66px;border-radius:4px;" />
            <div style="flex:1;">
              <el-skeleton-item variant="text" style="width:60%;" />
              <el-skeleton-item variant="text" style="width:40%;margin-top:8px;" />
              <el-skeleton-item variant="text" style="width:30%;margin-top:8px;" />
            </div>
          </div>
        </template>
        <template #default>
          <ul>
            <li v-if="!loadingProducts && products.length === 0" class="empty">暂无商品，换个条件试试～</li>
            <li v-for="product in products" :key="product.id" tabindex="0">
          <img :src="product.productPhoto?.productPhotoUrl || '/img/dealerImgs/picUser.png'" alt="" />
          <div>
            <p class="p-name">{{ product.productName }}</p>
            <p class="p-type">类别：{{ product.typeName }}</p>
            <p class="p-price">
              &yen;{{ transPrice(product.productSale?.salePrice) }}/张
            </p>
            <p class="p-cart" role="button" aria-label="加入购物车" @click.stop="onAddCart(product.productNo, 1)">
              <em></em>
              <i>x1</i>
            </p>
          </div>
        </li>
          </ul>
        </template>
      </el-skeleton>
      <!-- 左边物品类型面板 -->
      <div :class="['left-menu', { 'left-menu-show': isShowLeft }]">
        <div class="left-switch" @click="showLeft()">
          <img src="/img/dealerImgs/up.png" alt="" />
        </div>
        <ul>
          <li
            v-for="productType in productTypes"
            :key="productType.id"
            :class="{ 'left-item-select': typeSelected == productType.typeNo }"
            @click="selectType(productType.typeNo)"
          >
            {{ productType.typeName }}
          </li>
        </ul>
      </div>
    </div>
    <!-- 右边物品属性面板 -->
    <div class="right-pad">
      <div class="list-pad">
        <ul class="f-type-list">
          <template v-for="(values, key) in productProps">
            <li v-if="values.length > 0" :key="key">
              <p>{{ GetPropKey(String(key), "1") }}</p>
              <ul class="f-item-list">
                <li
                  v-for="value in values"
                  :key="value"
                  @click="selectProp(GetPropKey(String(key), '0'), value)"
                >
                  <span
                    :class="{
                      'prop-select': propSelect[GetPropKey(String(key), '0')] == value,
                    }"
                    >{{ value }}</span
                  >
                </li>
              </ul>
              <div class="clear-tag"></div>
            </li>
          </template>
        </ul>
      </div>
      <div class="right-edit">
        <button
          @click="confirmFilter()"
          style="background-color: rgb(188, 0, 0); color: #fff"
        >
          确定
        </button>
        <button @click="hideRight()">取消</button>
      </div>
    </div>
    <div class="cover" v-show="isShowCover" @click="hideRight()"></div>
  </div>
</template>

<script lang="ts">
import { defineComponent, onMounted, reactive, toRefs, watch } from "vue";
import { ElMessage } from 'element-plus'
import { transPrice } from "../utility/common";
import {
  getProduct,
  getBelogType,
  getType,
  getProp,
  addCart,
} from "../HttpRequests/ProductListRequest";
import {
  IProductInfo,
  IProduct,
  IBlongType,
  IProductType,
} from "../interfaces/ProductList";
import { useRoute, useRouter } from "vue-router";
// @ts-ignore
import { useStore } from "vuex";

export default defineComponent({
  name: 'ProductList',
  setup() {
    var store = useStore();
    var router = useRouter();
    var route = useRoute();

    const pageController = reactive({
      isShowLeft: false,
      isShowCover: false,
      isShowSearchBtn: false,
    });

    const productInfo: IProductInfo = reactive({
      systemIndex: "板材",
      searchText: "",
      propSelect: {},
      products: [],
  loadingProducts: false,
      belogTypes: [],
      productTypes: [],
      productProps: {},
      typeSelected: "",
      timer: null,
      pageIndex: 1,
      /**
       * 获取物品
       */
      getPruducts: async (
        belongTypeName: string,
        productType: string | null,
        searchText: string | null,
        productProps: string | null
      ) => {
        try {
          productInfo.loadingProducts = true;
          const products = (await getProduct({
            searchText,
            belongTypeName,
            productType,
            productProps,
            sort: "ProductName",
            pageIndex: productInfo.pageIndex,
          })) as IProduct[];
          products.forEach((p) => {
            productInfo.products.push(p);
          });
        } catch (e) {
          ElMessage.error('加载商品失败，请稍后再试')
        } finally {
          productInfo.loadingProducts = false;
        }
      },
      getBelongTypes: async () => {
        try {
          productInfo.belogTypes = (await getBelogType()) as IBlongType[];
        } catch (e) {
          productInfo.belogTypes = []
          ElMessage.error('加载商品大类失败')
        }
      },
      /**
       * 点击大类时我们不需要考虑搜索的内容，因为每次点击大类，都应该清空搜索框
       * 但是所搜物品时就应该考虑大类
       */
      selectSystemProduct: async (belongTypeName: string) => {
        productInfo.propSelect = {};
        productInfo.typeSelected = "";
        productInfo.searchText = "";
        router.push(`/layoutMain/productList?belongtype=${belongTypeName}`);
      },
      /**
       * 从后端获取物品类型
       */
      getType: async (belongTypeName: string) => {
        try {
          productInfo.productTypes = (await getType(
            belongTypeName
          )) as IProductType[];
        } catch (e) {
          productInfo.productTypes = []
        }
      },
      /**
       * 选择物品类型，选择物品类型时可以清空搜索栏
       */
      selectType: async (typeNo: string) => {
        productInfo.propSelect = {};
        productInfo.searchText = "";
        if (productInfo.typeSelected == typeNo) {
          productInfo.typeSelected = "";
        } else {
          productInfo.typeSelected = typeNo;
        }
        setRouter();
      },
      /**
       * 从后端获取物品属性
       */
      getProps: async (belongTypeName: string, typeNo: string | null) => {
        try {
          var res = await getProp({ belongTypeName, typeNo });
          productInfo.productProps = res;
        } catch (e) {
          productInfo.productProps = {}
        }
      },

      /**
       * 获取物品属性种类的名称
       */
      GetPropKey: (key: string, index: number) => {
        const parts = key.split("|");
        if (index === 0) return parts[0] ?? key;
        return parts[1] ?? parts[0] ?? key;
      },
      /**
       * 搜索物品
       */
      search: () => {
        if (productInfo.timer) {
          clearTimeout(productInfo.timer);
        }
        productInfo.timer = setTimeout(async () => {
          setRouter();
        }, 1000);
      },
      /**
       * 选择属性
       */
      selectProp: (propKey: string, propValue: string) => {
        if (productInfo.propSelect[propKey] == propValue) {
          productInfo.propSelect[propKey] = "";
        } else {
          productInfo.propSelect[propKey] = propValue;
        }
        // console.log(productInfo.propSelect)
      },
      /**
       * 确认筛选
       */
      confirmFilter: () => {
        setRouter();
      },
      onAddCart: async (productNo: string, productNum: number) => {
        const customerNo = localStorage["cno"];
        var currentCartNum = store.getters["shoppingCart/getCartNum"];
        store.dispatch("shoppingCart/setCartNum", currentCartNum + 1);
        try {
          await addCart({
            customerNo,
            productNo,
            productNum,
          });
          ElMessage.success('已加入购物车')
        } catch {
          ElMessage.error('加入购物车失败')
        }
      },
    });
    /**
     * 将选中的物品属性转化成字符串
     */
    const productPropToString = () => {
      // 属性拼接字符串清空
      producProps = "";
      for (const key in productInfo.propSelect) {
        const value = productInfo.propSelect[key];
        if (value != "") producProps += `${key}_${value}^`;
      }
      producProps = producProps.substring(0, producProps.length - 1);
      console.log(producProps);
    };
    /**
     * 设置路由
     */
    const setRouter = () => {
      // 根地址，包含根路由以及物品大类信息
  var url = `/layoutMain/productList?belongtype=${productInfo.systemIndex}`;
      // 拼接物品搜索信息
      if (productInfo.searchText?.trim() != "") {
        url += `&keywords=${productInfo.searchText}`;
      }
      // 拼接物品类型
      if (productInfo.typeSelected?.trim() != "") {
        url += `&type=${productInfo.typeSelected}`;
      }
      // 拼接属性
      productPropToString();
      if (producProps != "") {
        url += `&prop=${producProps}`;
      }
      router.push(url);
    };
    const showLeft = () => {
      pageController.isShowLeft = !pageController.isShowLeft;
    };

    const searchFocus = () => {
      pageController.isShowSearchBtn = true;
    };

    const searchBlur = () => {
      pageController.isShowSearchBtn = false;
    };

    const showRight = () => {
      pageController.isShowCover = true;
      // var dom = document.querySelectorAll('.right-pad') as NodeListOf<HTMLElement>
      var dom = document.querySelector(".right-pad") as HTMLElement;
      dom.style.right = "0";
    };

    const hideRight = () => {
      pageController.isShowCover = false;
      var dom = document.querySelector(".right-pad") as HTMLElement;
      dom.style.right = "-85%";
    };

    let keywords: string = "";
    let systemIndex: string = "";
    let producType: string = "";
    let producProps: string = "";
    /**
     * 解析地址
     */
    const resolutionAddress = () => {
      productInfo.searchText = keywords =
        (route.query.keywords as string) ?? "";
      productInfo.systemIndex = systemIndex =
        (route.query.belongtype as string) ?? "板材";
      productInfo.typeSelected = producType =
        (route.query.type as string) ?? "";

      producProps = (route.query.prop as string) ?? "";
      // 格式大约如是：&prop=xxx_xx1||xxx_xx2^yyy_yyy
      if (producProps != "") {
        var arrayProducProps = producProps.split("^");
        for (let i = 0; i < arrayProducProps.length; i++) {
          const element: string = arrayProducProps[i];
          productInfo.propSelect[element.split("_")[0]] = element.split("_")[1];
        }
        console.log(productInfo.propSelect);
      }

      console.log(systemIndex);
    };
    /**
     * 监听页面的滚动事件
     */
    const handleScroll = () => {
      var htmlDom = document.querySelector("html") as HTMLElement;
      // 1.获取当前整个页面长度
      var htmlHeight = htmlDom.offsetHeight;
      // 2.获取滚动条距离顶部的距离
      var scrollTop = htmlDom.scrollTop;
      // 3.获得屏幕可视区域的高度
      var screenHeight = document.documentElement.clientHeight;
      // 4.获取可视区域底部到整个页面底部的距离
      var diffHeight = htmlHeight - scrollTop - screenHeight;
      if (diffHeight == 0 && scrollTop > 0) {
        onPageChange();
      }
    };

    const onPageChange = async () => {
      productInfo.pageIndex++;
      await productInfo.getPruducts(
        systemIndex,
        producType,
        keywords,
        producProps
      );
    };
    onMounted(async () => {
      window.addEventListener("scroll", handleScroll);
      // 先取大类，再确定默认大类
      await productInfo.getBelongTypes();
      resolutionAddress();
      if (!systemIndex || systemIndex.trim() === '') {
        systemIndex = productInfo.belogTypes?.[0]?.belongTypeName || ''
        productInfo.systemIndex = systemIndex
      }
      await productInfo.getType(systemIndex);
      await productInfo.getProps(systemIndex, producType);
      productInfo.products = []
      productInfo.pageIndex = 1
      await productInfo.getPruducts(
        systemIndex,
        producType,
        keywords,
        producProps
      );
    });

    // 监听路由查询参数变化，重置并重新加载
    watch(
      () => route.fullPath,
      async () => {
        resolutionAddress();
        // 路由参数变化时，刷新类型与属性面板
        await productInfo.getType(systemIndex)
        await productInfo.getProps(systemIndex, producType)
        // 重置并加载列表
        productInfo.products = []
        productInfo.pageIndex = 1
        await productInfo.getPruducts(
          systemIndex,
          producType,
          keywords,
          producProps
        );
      }
    )

    return {
      ...toRefs(pageController),
  ...toRefs(productInfo),
      transPrice,
      showLeft,
      searchFocus,
      searchBlur,
      showRight,
      hideRight,
      onSearchInput: productInfo.search,
      selectSystemProduct: (belongTypeName: string) => {
        productInfo.systemIndex = belongTypeName;
        setRouter();
      },
      onAddCart: productInfo.onAddCart,
  // 注：原“点击商品跳详情”是后续新增功能。按当前需求已移除该交互。
      selectType: (typeNo: string) => {
        productInfo.typeSelected = typeNo;
        setRouter();
      },
      GetPropKey: (key: string, flag: string) => {
        return key; // 直接返回key，不区分flag
      },
      selectProp: (propKey: string, value: string) => {
        productInfo.propSelect[propKey] = value;
      },
      confirmFilter: productInfo.confirmFilter
    };
  },
});
</script>

<style lang="scss" scoped>
.i-search:after {
  background-color: #b70101 !important;
}

.search-pad {
  z-index: 10;
  position: fixed;
  width: 100%;
  padding: 6px 20px;
  background-color: #f0f0f0;
  display: flex;

  input {
    height: 28px;
    box-sizing: border-box;
    border: 1px solid #ddd;
    border-radius: 3px;
    flex: 1;
    outline: none;
  }

  button {
    background-color: transparent;
    width: 56px;
    border: 0 none;
    font-size: 14px;
    font-weight: bold;
    color: #333;
    outline: none;
  }
}

.system-pad {
  z-index: 10;
  background-color: #fff;
  display: flex;
  position: fixed;
  top: 40px;
  width: 100%;
  .system-item {
    flex: 1;
    text-align: center;
    border-bottom: 1px #ddd solid;
    border-right: 1px transparent solid;
    border-left: 1px transparent solid;

    span {
      border: 0 none !important;
      background-color: #f0f2f5;
      margin: 6px 5px;
      font-size: 12px;
      font-weight: normal;
      text-align: center;
      border-radius: 4px;
      padding: 6px 0;
      display: block;
      height: 22px;
      line-height: 12px;
    }
  }

  .system-select {
    border-bottom: 1px transparent solid;
    border-right: 1px #ddd solid;
    border-left: 1px #ddd solid;

    span {
      background-color: transparent;
    }
  }
}

.product-list {
  padding-top: 75px;
  ul {
    background-color: #fff;
    .empty {
      text-align: center;
      padding: 24px 0;
      color: #666;
      font-size: 13px;
    }

    li {
      list-style: none;
      height: 88px;
      padding-left: 108px;
      position: relative;

      img {
        height: 66px;
        width: 66px;
        background-color: #ccc;
        position: absolute;
        left: 28px;
        top: 11px;
      }

      div {
        overflow: hidden;
        padding: 10px 0;
        border-bottom: 1px solid #f0f0f0;
        padding-bottom: 6px;
        text-align: left;
        .p-name {
          font-size: 13px;
        }

        .p-type {
          font-size: 12px;
          color: #666;
          margin-top: 8px;
        }

        .p-price {
          font-size: 13px;
          color: #f23030;
          margin-top: 8px;
        }
        .p-cart {
          position: relative;
          float: right;
          background-color: #b70101;
          height: 20px;
          width: 40px;
          background-image: url("/img/icons-png/shoppingCar-white.png");
          background-repeat: no-repeat;
          background-position: center;
          background-position-x: 45%;
          background-size: 16px;
          border-radius: 50px;
          margin-right: 26px;
          i {
            position: absolute;
            right: -18px;
            font-size: 12px;
            top: 3px;
          }
        }
      }
    }
  }

  .left-menu {
    position: fixed;
    height: calc(100% - 116px);
    left: -106px;
    width: 125px;
    background-color: #fff;
    top: 76px;
    border-radius: 0 18px 0 0;
    border: 1px solid #d7d7d7;
    overflow: hidden;
    transition: 0.5s;
    margin-bottom: 120px;

    .left-switch {
      width: 20px;
      background-color: #fff;
      position: absolute;
      right: 0;
      height: 100%;

      img {
        position: absolute;
        top: 42%;
        left: 2px;
        width: 20px;
        transform: rotate(90deg);
        transition: 0.5s;
      }
    }

    ul {
      position: absolute;
      height: 100%;
      width: 106px;
      background-color: #f0f0f0;
      overflow: auto;

      li {
        width: 106px;
        height: 50px;
        text-align: center;
        line-height: 50px;
        border-bottom: 1px solid #d7d7d7;
        padding: 0;
        font-size: 12px;
        color: #333;
      }

      li.left-item-select {
        background-color: #fff;
      }
    }
  }

  .left-menu-show {
    left: 0;

    .left-switch {
      img {
        transform: rotate(-90deg);
      }
    }
  }
}

.right-pad {
  position: fixed;
  /* right: -85%; */
  right: -85%;
  top: 0;
  width: 85%;
  height: 100%;
  background-color: #f7f7f7;
  z-index: 103;
  transition: 580ms;
  z-index: 101;

  ul {
    list-style: none;
    overflow: hidden;
  }
  .list-pad {
    overflow: auto;
    height: 100%;
    padding-bottom: 40px;
    .f-type-list {
      overflow: hidden;

      > li {
        padding: 10px;
        background-color: #fff;
        margin-bottom: 10px;

        .f-item-list {
          overflow: hidden;
          display: flex;
          flex-wrap: wrap;

          li {
            flex-basis: 33.3%;

            span {
              display: block;
              margin-top: 10px;
              margin-right: 10px;
              background: #eee;
              border: 1px solid #eee;
              padding: 5px 0;
              text-align: center;
              border-radius: 6px;
              font-size: 13px;
              overflow: hidden;
              height: 29px;
              line-height: 22px;
            }

            .prop-select {
              border: 1px solid red;
              background: #fff;
              color: red;
            }
          }
        }

        p {
          font-size: 14px;
        }
      }
    }
  }
  .right-edit {
    position: absolute;
    bottom: 0;
    left: 0;
    width: 100%;

    button {
      float: left;
      height: 40px;
      width: 50%;
      line-height: 40px;
      text-align: center;
      border: 0px none;
    }
  }
}

.cover {
  position: fixed;
  z-index: 11;
  height: 100%;
  width: 100%;
  left: 0;
  top: 0;
  background-color: rgba(51, 51, 51, 0.36);
}
</style>
