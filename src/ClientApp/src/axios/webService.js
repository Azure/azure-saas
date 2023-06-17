import axios from "axios";
import { getLocalData } from "../helpers/auth";

const BASE_URL =
  process.env.REACT_APP_BASE_URL + process.env.REACT_APP_API_VERSION;
const service = axios.create({ baseURL: BASE_URL });

export default class WebService {
  static async post(action, params) {
    let response = await service.post(action, params);
    return response.data;
  }
  static async put(action, params) {
    let response = await service.put(action, params);
    return response.data;
  }
  static async get(action) {
    let response = await service.get(action);
    return response.data;
  }
  static async delete(action) {
    let response = await service.delete(action);
    return response.data;
  }
  static async patch(action, params) {
    let response = await service.patch(action, params);
    return response.data;
  }
}

service.interceptors.request.use(
  async (config) => {
    // Do something before request is sent
    config.baseURL = BASE_URL;
    const token = await getLocalData("token");

    if (token) {
      config.headers = { Authorization: `Bearer ${token}` };
    }

    return config;
  },
  (error) => {
    // Do something with request error
    return Promise.reject(error);
  }
);

service.interceptors.response.use(
  (response) => {
    // Any status code that lie within the range of 2xx cause this function to trigger
    // Do something with response data

    return response;
  },
  (error) => {
    // // Any status codes that falls outside the range of 2xx cause this function to trigger
    // // Do something with response error

    const { response } = error;
    // const originalRequest = config;

    if (response.status === 401 || response.status === 404) {
      return Promise.reject(error);
    } else {
      return Promise.reject(error);
    }
  }
);
