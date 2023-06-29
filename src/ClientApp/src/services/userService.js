import { loginSuccess } from "../redux/userSlice";
import { setUpToken } from "../helpers/auth";
import Constant from "../utils/constant";
import OnboardingService from "../axios/onboardingRequest";

export const getCurrentUser = async (dispatch) => {
  const action = Constant.ACTION.USER;

  try {
    const response = await OnboardingService.get(action);
    setUpToken(response?.accessToken);
    dispatch(loginSuccess(response));
  } catch (error) {
    console.log(error);
  }
};
