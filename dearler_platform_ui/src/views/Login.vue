<template>
  <div>
    <div class="login-pad">
  <h2 style="font-weight:600;letter-spacing:2px;">经销商平台</h2>
      <p>
        <el-input v-model="userNo" placeholder="用户账号" size="large" clearable />
      </p>
      <p>
        <el-input v-model="password" placeholder="密码" show-password size="large" @keyup.enter="handleLogin" />
      </p>
      <el-button type="primary" size="large" round style="width: 100%;" @click="login">登录</el-button>
      <!-- <router-link to="/Home" v-slot="{ navigate }" custom>
        <button @click="navigate">
          →
        </button>
      </router-link> -->
    </div>
    <div class="login-bottom">
      &copy;全栈ACE
    </div>
  </div>
</template>

<script>
import { reactive, toRefs } from 'vue'
import { ElMessage } from 'element-plus'
import { login } from '../HttpRequests/LoginRequest'
import { useRouter } from 'vue-router'
export default {
  setup () {
    var router = useRouter();
    const loginInfo = reactive({
      userNo: '',
      password: '',
      login: async () => {
        if (!loginInfo.userNo || !loginInfo.password) {
          ElMessage.error('请输入账号和密码')
          return
        }
        try {
          var res = await login({
            customerNo: loginInfo.userNo,
            password: loginInfo.password
          })
          if (res) {
            localStorage['cno'] = loginInfo.userNo;
            localStorage['token'] = res;
            router.push("/layoutMain");
          }
        } catch (err) {
          ElMessage.error('登录失败，请检查账号或密码')
        }
      },
      // 兼容回车提交
      handleLogin: async () => {
        await loginInfo.login()
      }
    })

    return { ...toRefs(loginInfo) }
  }
}
</script>

<style lang="scss" scoped>
.login-pad {
  text-align: center;
  width: 60%;
  margin: auto;
  margin-top: 26%;
  h2 {
  font-weight: 600;
    margin-bottom: 30px;
  }
  p {
    width: 100%;
    margin-top: 20px;
    input {
      width: 100%;
      box-sizing: border-box;
      height: 36px;
      border-radius: 18px;
      border: 0 none;
      background-color: #f0f0f0;
      text-align: center;
    }
  }
  button {
    margin-top: 36px;
    width: 60px;
    height: 60px;
    border-radius: 30px;
    border: 0 none;
    background-color: rgb(79, 137, 245);
    color: #fff;
    font-size: 26px;
    font-weight: bold;
  }
}
.login-bottom {
  position: absolute;
  bottom: 10px;
  text-align: center;
  width: 100%;
  font-size: 14px;
}
</style>
