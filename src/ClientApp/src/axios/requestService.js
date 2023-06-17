import axios from "axios";

const BASE_URL =
  process.env.REACT_APP_BASE_URL + process.env.REACT_APP_API_VERSION;

const requestService = axios.create({ baseURL: BASE_URL });

requestService.interceptors.request.use(
  (config) => {
    const TOKEN = localStorage.getItem("token");

    if (TOKEN) {
      config.headers = { Authorization: `Bearer ${TOKEN}` };
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export default requestService;
