import { createSlice } from "@reduxjs/toolkit";

const moduleSlice = createSlice({
  name: "moduleCategory",
  initialState: {
    partitionKey: "",
  },
  reducers: {
    getModule: (state, action) => {
      state.partitionKey = action.payload;
    },
  },
});

export const { getModule } = moduleSlice.actions;
export default moduleSlice.reducer;
