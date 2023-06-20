import { createSlice } from "@reduxjs/toolkit";
import { logoutFunc } from "../helpers/auth.js";

const userSlice = createSlice({
  name: "user",
  initialState: {
    currentUser: null,
    isFetching: false,
    error: false,
  },
  reducers: {
    loginStart: (state) => {
      state.isFetching = true;
    },
    loginSuccess: (state, action) => {
      state.isFetching = false;
      state.currentUser = action.payload;
    },
    loginFailure: (state) => {
      state.isFetching = false;
      state.error = true;
    },
    logout: (state) => {
      state.currentUser = null;
      logoutFunc();
    },
    updateUserProfile: (state, action) => {
      state.currentUser = {
        ...state.currentUser,
        user: action.payload.user,
      };
    },
  },
});

export const {
  loginStart,
  loginSuccess,
  loginFailure,
  logout,
  updateUserProfile,
} = userSlice.actions;
export default userSlice.reducer;
