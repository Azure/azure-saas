import axios from "axios";

const ErpService = axios.create({
  baseURL: "http://localhost:5066", // Replace with your API's base URL
  withCredentials: true, // Include credentials in the request
});

export default ErpService;
// https://saas-app-asdk-test-576y.azurewebsites.net
