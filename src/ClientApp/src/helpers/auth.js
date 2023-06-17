import axios from "axios";
import Constant from "../utils/constant";

export const logoutFunc = () => {
  window.location = process.env.REACT_APP_SIGNOUT_URL;
  localStorage.removeItem("token");
};

export const setUpToken = (token) => {
  localStorage.setItem("token", token);
};

export function setLocalData(key, value) {
  try {
    localStorage.setItem(key, value);
  } catch (error) {
    console.log("error", error);
  }
}
export function removeLocalData(key) {
  try {
    localStorage.removeItem(key);
  } catch (error) {
    console.log("error", error);
  }
}

export function getLocalData(key) {
  try {
    let data = localStorage.getItem(key);
    return data;
  } catch (error) {
    console.log("error", error);
  }
}

export async function getCRSFToken() {
  const url =
    process.env.REACT_APP_BASE_URL +
    process.env.REACT_APP_API_VERSION +
    Constant.ACTION.CSRFTOKEN;

  try {
    const { data } = await axios.get(url, { withCredentials: true });
    setLocalData("csrfToken", data?.token);
  } catch (error) {
    console.error("Failed to fetch CSRF token:", error);
  }
}
