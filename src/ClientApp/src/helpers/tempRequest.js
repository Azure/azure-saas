import axios from "axios";

const BASE_URL = "https://ibusinessaccountservice.azurewebsites.net/api";

let request = axios.create({ baseURL: BASE_URL });

request.interceptors.request.use(
  (config) => {
    const TOKEN = localStorage.getItem("token");

    if (TOKEN) {
      config.headers = {
        Authorization: `Bearer ${TOKEN}`,
        "Access-Control-Allow-Origin":
          "https://ibusiness-git-main-moryno.vercel.app",
      };
    }

    return config;
  },
  (error) => {
    console.log(error);
  }
);

export default request;

export const msSingleSign =
  "https://bookingapptrial.azurewebsites.net/login/Loginwithmicrosoft";
// "https://login.microsoftonline.com/429eb2c8-ad5d-4e03-b326-a26d27a067f7/oauth2/authorize?client_id=fd6d9002-4eb4-4b56-b3a4-29f9cf05141f&response_type=token&redirect_uri=http://localhost:3000/login&resource=fd6d9002-4eb4-4b56-b3a4-29f9cf05141f&scope=openid&response_mode=fragment&state=12345&nonce=678910";
