import { Copyright } from "../components/frontend/copyright/Copyright";
import { Footer } from "../components/frontend/footer/Footer";
import { Navbar } from "../components/frontend/navbar/Navbar";

const FrontendLayout = ({ children }) => {
  return (
    <main>
      <Navbar />
      {children}
      <Footer />
      <Copyright />
    </main>
  );
};

export default FrontendLayout;
