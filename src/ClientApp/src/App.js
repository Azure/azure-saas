import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import "./assets/styles.css";

import Profile from "./pages/dashboard/users/Profile";
import Home from "./pages/dashboard/Home";
import { PurchaseOrder } from "./pages/dashboard/purchase-orders/PurchaseOrder";
import Orders from "./pages/dashboard/Orders";
import { LandingPage } from "./pages/landing-page/LandingPage";
import { Company } from "./pages/landing-page/Company";
import { ProductsView } from "./pages/landing-page/ProductsView";
import { Support } from "./pages/landing-page/Support";
import Onboarding from "./pages/landing-page/Onboarding";
import FrontendLayout from "./layout/FrontendLayout";
import Booking from "./pages/dashboard/Booking";
import ProtectedRoute from "./components/dashboard/ProtectedRoute";
import User from "./pages/dashboard/users/User";
import ScrollToTop from "./components/frontend/ScrollToTop";
import { NotFound } from "./pages/landing-page/NotFound";

function App() {
  return (
    <Router>
      <ScrollToTop>
        <Routes>
          <Route path="/">
            <Route
              index
              element={
                <FrontendLayout>
                  <LandingPage />
                </FrontendLayout>
              }
            ></Route>
            <Route
              path="pricing"
              element={
                <FrontendLayout>
                  <ProductsView />
                </FrontendLayout>
              }
            ></Route>
            <Route
              path="company"
              element={
                <FrontendLayout>
                  <Company />
                </FrontendLayout>
              }
            ></Route>
            <Route
              path="support"
              element={
                <FrontendLayout>
                  <Support />
                </FrontendLayout>
              }
            ></Route>
            <Route
              path="onboarding"
              element={
                <FrontendLayout>
                  <Onboarding />
                </FrontendLayout>
              }
            ></Route>
          </Route>
          <Route path="dashboard">
            <Route
              index
              element={
                <ProtectedRoute>
                  <Home />
                </ProtectedRoute>
              }
            ></Route>
            <Route
              path="users"
              element={
                <ProtectedRoute>
                  <User />
                </ProtectedRoute>
              }
            ></Route>

            <Route
              path="bookings"
              element={
                <ProtectedRoute>
                  <Booking />
                </ProtectedRoute>
              }
            ></Route>

            <Route
              path="purchase-order"
              element={
                <ProtectedRoute>
                  <PurchaseOrder orderstate={0} />
                </ProtectedRoute>
              }
            ></Route>
            <Route
              path="profile"
              element={
                <ProtectedRoute>
                  <Profile />
                </ProtectedRoute>
              }
            ></Route>
            <Route
              path="orders"
              element={
                <ProtectedRoute>
                  <Orders />
                </ProtectedRoute>
              }
            ></Route>
            <Route
              path="updateorder/:id"
              element={
                <ProtectedRoute>
                  <PurchaseOrder orderstate={1} />
                </ProtectedRoute>
              }
            ></Route>
          </Route>
          <Route
            path="*"
            element={
              <FrontendLayout>
                <NotFound />
              </FrontendLayout>
            }
          />
        </Routes>
      </ScrollToTop>
    </Router>
  );
}
export default App;
