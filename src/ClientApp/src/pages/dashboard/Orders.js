import { useEffect, useState, memo, useRef } from "react";
import { useNavigate } from "react-router-dom";
import { orderColumns } from "../../data/PurchaseOrderData";
import DataTable from "../../components/dashboard/DataTable";
import Statusbar from "../../components/dashboard/Statusbar";
import MenuButtonsGroup from "../../components/dashboard/MenuButtonsGroup";
import { homeMenuSource } from "../../data/menu";
import MobileMenus from "../../components/dashboard/MobileMenus";
import request from "../../helpers/tempRequest";
import { orderFilterValues } from "../../helpers/datatableSource";

const Orders = () => {
  const [data, setData] = useState([]);
  const loadingRef = useRef(false);
  // eslint-disable-next-line
  const [onRowClickItem, setRowClickItem] = useState(null);
  const [onRowDblClickBookingId, setRowDblClickBookingId] = useState(null);
  const [input, setInput] = useState(null);
  const navigate = useNavigate();

  const today = new Date().toISOString().slice(0, 10);

  const handleChange = (event) => {
    const { name, value } = event.target;
    setInput({ ...input, [name]: value });
  };

  useEffect(() => {
    const getData = async () => {
      try {
        const response = await request.get("PurchaseOrder/orders");
        setData(response.data);
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
      navigate(`/dashboard/updateorder/${onRowDblClickBookingId}`);
    }
  }, [onRowDblClickBookingId, navigate]);

  const handleClick = (menu) => {
    switch (menu) {
      case "Find":
        break;
      case "New":
        navigate("/dashboard/purchase-order");
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
    <main className="w-full min-h-full relative  px-3 md:px-5 py-1.5">
      <section>
        <section>
          <MenuButtonsGroup
            heading="Purchase Orders"
            menus={homeMenuSource}
            onMenuClick={handleClick}
          />

          <article className="relative">
            <MobileMenus menus={homeMenuSource} onMenuClick={handleClick} />

            <article className=" md:pr-5 flex gap-4 items-center">
              <div className="flex flex-col gap-2 md:flex-row w-full md:py-2">
                <div className="flex w-full justify-between md:justify-start md:w-1/2 items-center gap-5">
                  <label
                    className="font-semibold text-xs  text-gray-600"
                    htmlFor="fromDate"
                  >
                    From Date:
                  </label>
                  <input
                    className="border text-sm border-gray-300 w-1/2 outline-none rounded-sm"
                    type="date"
                    id="fromDate"
                    onChange={handleChange}
                    name="startdate"
                    defaultValue={today}
                  />
                </div>
                <div className="flex w-full justify-between md:justify-start md:w-1/2 items-center gap-5">
                  <label
                    className="font-semibold text-xs  text-gray-600"
                    htmlFor="toDate"
                  >
                    To Date:
                  </label>
                  <input
                    className="border text-sm border-gray-300 w-1/2 outline-none rounded-sm"
                    type="date"
                    id="toDate"
                    onChange={handleChange}
                    name="enddate"
                    defaultValue={today}
                  />
                </div>
              </div>
            </article>
          </article>
        </section>

        <section className="mt-5">
          <DataTable
            data={data}
            columns={orderColumns}
            keyExpr="orderNumber"
            startEdit={startEdit}
            loading={loadingRef.current}
            setRowClickItem={setRowClickItem}
            filterValues={orderFilterValues}
          />
        </section>
      </section>
      <Statusbar heading="Purchase Orders" company="iBusiness" />
    </main>
  );
};

export default memo(Orders);
