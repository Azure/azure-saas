import axios from "axios";
import { getLocalData } from "../helpers/auth";

const BASE_URL =
  process.env.REACT_APP_BASE_URL + process.env.REACT_APP_API_VERSION;

const onboardingRequest = axios.create({
  baseURL: BASE_URL,
  withCredentials: true,
});

//export default onboardingRequest;

export default class OnboardingService {
  static async post(action, params) {
    let response = await onboardingRequest.post(action, params);
    return response.data;
  }
  static async put(action, params) {
    let response = await onboardingRequest.put(action, params);
    return response.data;
  }
  static async get(action) {
    let response = await onboardingRequest.get(action);
    return response.data;
  }
  static async delete(action) {
    let response = await onboardingRequest.delete(action);
    return response.data;
  }
  static async patch(action, params) {
    let response = await onboardingRequest.patch(action, params);
    return response.data;
  }
}

onboardingRequest.interceptors.request.use(
  async (config) => {
    const csrfToken = getLocalData("csrfToken");

    if (csrfToken) {
      config.headers = { "X-Csrf-Token": csrfToken };
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

onboardingRequest.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    const { response } = error;
    console.log(error);
    if (response.status === 401 || response.status === 404) {
      return Promise.reject(error);
    } else {
      return Promise.reject(error);
    }
  }
);
