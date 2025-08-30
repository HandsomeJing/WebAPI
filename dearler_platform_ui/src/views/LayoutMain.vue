<template>
  <div>
    <router-view :key="route.fullPath" />
    <div class="foot-menu-pad">
      <div class="foot-menu">
        <router-link to="/layoutMain/productList" v-slot="{ navigate }" custom>
          <div class="foot-item" @click="navigate">
            <b :class="['i-search', { 'f-menu-sel': route.path.startsWith('/layoutMain/productList') }]"></b>
          </div>
        </router-link>
        <router-link to="/layoutMain/shoppingCart" v-slot="{ navigate }" custom>
          <div class="foot-item" @click="navigate">
            <b :class="['i-cart', { 'f-menu-sel': route.path.startsWith('/layoutMain/shoppingCart') }]">
              <i>{{ store.getters["shoppingCart/getCartNum"] }}</i>
            </b>
          </div>
        </router-link>
        <router-link to="/layoutMain/main" v-slot="{ navigate }" custom>
          <div class="foot-item" @click="navigate">
            <b :class="['i-user', { 'f-menu-sel': route.path.startsWith('/layoutMain/main') }]"></b>
          </div>
        </router-link>
      </div>
    </div>
  </div>
</template>

<script>
import { ref ,onMounted} from "vue";
import { useRoute } from "vue-router";
import { shoppingCartNum } from "../store";
import { useStore } from "vuex";
import { getCartNum } from '../HttpRequests/MainRequest'

export default {
  setup() {
    const route = useRoute();
    const store = useStore();
    const onGetCartNum = async () => {
      // shoppingCartNum.value = await getCartNum(cno)
      let cartNo = await getCartNum();
      // this.$stroe.dispatch() // used in vue2
      store.dispatch("shoppingCart/setCartNum", cartNo);
    };
    onMounted(async () => {
      await onGetCartNum();
    });
    return { route, shoppingCartNum, store };
  },
};
</script>

<style lang="scss" scoped>
.foot-menu-pad {
  height: 40px;
  .foot-menu {
    position: fixed;
    height: 40px;
    background-color: #fff;
    width: 100%;
    left: 0;
    bottom: 0;
    display: flex;

    .foot-item {
      flex: 1;
      text-align: center;
      height: 40px;
      line-height: 40px;
      position: relative;

      b {
        background-color: #f0f0f0;
        width: 28px;
        height: 28px;
        border-radius: 14px;
        margin-top: 6px;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        position: relative;
        font-size: 16px;

        i {
          position: absolute;
          font-size: 12px;
          color: #fff;
          background-color: red;
          padding: 1px 3px;
          text-align: center;
          font-style: normal;
          display: inline;
          top: -5px;
          right: -12px;
          border-radius: 12px;
          line-height: 12px;
          font-weight: normal;
        }
      }

  b.i-search::before { content: '🔍'; }
  b.i-cart::before { content: '🛒'; }
  b.i-user::before { content: '👤'; }
  .f-menu-sel { background-color: #b70101; color: #fff; }
    }
  }
}
</style>
