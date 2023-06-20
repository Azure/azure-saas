import React, { useState, useRef, useEffect } from "react";
import MenuButtonsGroup from "../../../components/dashboard/MenuButtonsGroup";
import "./PurchaseOrder.css";
import "devextreme/dist/css/dx.common.css";
import "devextreme/dist/css/dx.light.css";
import { purchaseOrderMenu } from "../../../data/menu";
import DataSource from "devextreme/data/data_source";
import Form from "../../../components/dashboard/PurchaseOrder/Form";
import { InputField } from "../../../components/dashboard/PurchaseOrder/InputField";
import { Table } from "../../../components/dashboard/PurchaseOrder/Table";
import { MessageDiv } from "../../../components/dashboard/PurchaseOrder/Message";
import { useNavigate, useParams } from "react-router-dom";
import request from "../../../helpers/tempRequest";
import Statusbar from "../../../components/dashboard/Statusbar";
import { useSelector } from "react-redux";
import { LoadPanel } from "devextreme-react/load-panel";

export const PurchaseOrder = ({ orderstate }) => {
  const [currentmessage, setMessage] = useState();
  const [order, setOrder] = useState(0);
  const [updateData, setUpdateData] = useState({
    formData: "",
    tableData: "",
  });
  const [initialRender, setInitialRender] = useState(true);
  const currentUser = useSelector((state) => state.user?.currentUser?.user);
  const [formUpdateData, setFormUpdateData] = useState([]);
  const { id } = useParams();
  const [loading, setLoading] = useState(false);
  // eslint-disable-next-line
  const [data, setData] = useState(new DataSource());
  const count = useRef(1);
  const [modalmessage, setModalMessage] = useState(
    "Are you sure you want to clear the table?"
  );
  const navigate = useNavigate();

  useEffect(() => {
    setLoading(false);
    if (orderstate === 0) {
      async function getData() {
        try {
          const response = await request.get(
            `/PurchaseOrder/getorderitems?userid=${currentUser?.email}`
          );
          data.store().clear();
          response.data.orderItems.map((item) => {
            return data.store().insert(item);
          });
          if (response.data.orderInformation.length === 0) {
            setInitialRender(false);
            setLoading(false);
          } else {
            setFormUpdateData(response.data.orderInformation[0]);
          }
          data.reload();
        } catch (e) {
          console.log(e);
        }
      }
      getData();
    } else if (orderstate === 1) {
      const getUpdateData = async () => {
        try {
          const response = await request.get(
            `/PurchaseOrder/getorder?orderNumber=${id}`
          );
          response.data.tableData.map((item) => {
            data.store().insert(item);
            return data.reload();
          });
          setFormUpdateData(response.data.formInfo);
          setUpdateData({ ...updateData, formData: response.data.formInfo });
          setOrder(response.data.formInfo.orderNumber);
        } catch (e) {
          console.log(e);
        }
      };

      getUpdateData();
    }

    const handleKeyUp = (e) => {
      if (e.code === "KeyS" && e.ctrlKey) {
        e.preventDefault();
        submitOrder();
        document.removeEventListener("keydown", handleKeyUp);
      }
    };

    document.addEventListener("keydown", handleKeyUp);

    return () => {
      document.removeEventListener("keydown", handleKeyUp);
    };
    // eslint-disable-next-line
  }, [orderstate]);

  const submitOrder = async () => {
    setLoading(true);
    try {
      if (orderstate === 0) {
        const user = {
          userid: currentUser?.email,
        };
        await request.post("/PurchaseOrder/createpurchaseorder", user);

        setMessage("Order submitted successfully.");
        setLoading(false);
      } else {
        const dataToUpdate = {
          formData: updateData.formData,
          tableData: data.store()._array,
        };
        await request.put("/PurchaseOrder/updateorder", dataToUpdate);
        setMessage("Order has been updated successfully.");
        setLoading(false);
      }
      setTimeout(() => {
        navigate("/dashboard/orders");
      }, 1500);
    } catch (e) {
      console.log(e);
      setLoading(false);
      return setMessage(e.response.data["error"]);
    }
  };

  const handleClick = (menu) => {
    switch (menu) {
      case "Submit Order":
        submitOrder();
        break;

      case "Close":
        navigate("/dashboard/orders");
        break;

      default:
        break;
    }
  };

  return (
    <main className="purchase-order-page w-full min-h-full relative h-full px-3 md:px-5 py-1.5">
      <section>
        <MenuButtonsGroup
          heading={
            orderstate === 0 ? "Purchase Order Entry" : "Update Purchase Order"
          }
          menus={purchaseOrderMenu}
          onMenuClick={handleClick}
        />
      </section>
      <div className="mt-3 w-full">
        <Form
          orderState={orderstate}
          formUpdateData={formUpdateData}
          initialRender={initialRender}
          orderstate={orderstate}
          setInitialRender={setInitialRender}
          setLoading={setLoading}
          setUpdateData={setUpdateData}
          updateData={updateData}
          setMessage={setMessage}
        />
      </div>
      <section className="mt-2">
        <div className="po-grid h-full mt-2">
          <div className="add-item-btns">
            <div className="add-item">
              <InputField
                data={data}
                count={count}
                order={order}
                setMessage={setMessage}
                orderstate={orderstate}
              />
              <MessageDiv message={currentmessage} />
            </div>
          </div>
          <Table
            data={data}
            count={count}
            setMessage={setMessage}
            setModalMessage={setModalMessage}
            order={order}
            updateData={updateData}
            setUpdateData={setUpdateData}
            orderstate={orderstate}
          />
          <LoadPanel
            visible={loading}
            messageText="Checking for unsubmitted orders..."
          />
        </div>
      </section>
      <div id="confirm-modal" className="po-modal">
        <div className="po-modal-content">
          <div className="po-modal-header"></div>
          <div className="po-modal-body">
            <p>{modalmessage}</p>
          </div>
          <div className="po-modal-footer">
            <button id="confirm-yes" className="po-modal-btn">
              Yes
            </button>
            <button id="confirm-no" className="po-modal-btn">
              No
            </button>
          </div>
        </div>
      </div>
      <Statusbar heading="Purchase Order Entry" company="iBusiness" />
    </main>
  );
};
