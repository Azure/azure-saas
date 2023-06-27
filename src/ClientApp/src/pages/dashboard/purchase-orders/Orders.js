import { useEffect, useState, useRef } from "react";
import { useNavigate } from "react-router-dom";
import { orderColumns } from "../../../data/PurchaseOrderData";
import { homeMenuSource } from "../../../data/menu";
import { orderFilterValues } from "../../../helpers/datatableSource";
import CategoryComponent from "../../../components/dashboard/CategoryComponent";
import { toast } from "react-toastify";
import OnboardingService from "../../../axios/onboardingRequest";
import Constant from "../../../utils/constant";
import FromToDateComponent from "../../../components/dashboard/FromToDateComponent";
import DataTable from "../../../components/dashboard/DataTable";
import Statusbar from "../../../components/dashboard/Statusbar";
import MenusGroupComponent from "../../../components/dashboard/Menus/MenusGroupComponent";

const Orders = () => {
  const [data, setData] = useState([]);
  const loadingRef = useRef(false);
  // eslint-disable-next-line
  const [onRowClickItem, setRowClickItem] = useState(null);
  const [onRowDblClickBookingId, setRowDblClickBookingId] = useState(null);

  const navigate = useNavigate();
  const route = Constant.ROUTE.ORDER;

  useEffect(() => {
    const getData = async () => {
      try {
        const url = "/test2";
        const response = await OnboardingService.get(url);
        setData(response);
        loadingRef.current = false;
      } catch (error) {
        console.log(error);
      }
    };

    getData();
  }, []);

  const startEdit = ({ data }) => {
    if (data) {
      setRowDblClickBookingId(data.orderNumber);
    } else {
      setRowDblClickBookingId(null);
    }
  };

  useEffect(() => {
    if (onRowDblClickBookingId) {
      navigate(`/dashboard/orders/${onRowDblClickBookingId}/view`);
    }
  }, [onRowDblClickBookingId, navigate]);

  const openConfirmationPopup = async (rowItem) => {
    if (rowItem === null) {
      toast.warning("Please select a booking to delete");
    }
  };

  const handleClick = (menu) => {
    switch (menu) {
      case "Find":
        break;
      case "New":
        navigate("/dashboard/orders/new");
        break;
      case "Delete":
        console.log("Delete was clicked");
        break;
      case "Close":
        navigate("/dashboard");
        break;
      case "Help":
        console.log("Help was clicked");
        break;

      default:
        break;
    }
  };

  return (
    <main className="w-full min-h-full relative px-3 md:px-5 py-1.5">
      <section>
        <CategoryComponent>
          <MenusGroupComponent
            menus={homeMenuSource}
            heading={"Purchase Orders"}
            onMenuClick={handleClick}
          />
          <FromToDateComponent />
          <DataTable
            data={data}
            route={route}
            keyExpr={"orderNumber"}
            columns={orderColumns}
            startEdit={(e) => startEdit(e)}
            setRowClickItem={setRowClickItem}
            openConfirmationPopup={openConfirmationPopup}
            filterValues={orderFilterValues}
          />
          <Statusbar
            heading={"Purchase Orders"}
            company={"ARBS Customer Portal"}
          />
        </CategoryComponent>
      </section>
    </main>
  );
};

export default Orders;
