import { useEffect, useState } from "react";
import DateBox from "devextreme-react/date-box";
import { toast } from "react-toastify";
import DataTable from "../../components/dashboard/DataTable";
import Statusbar from "../../components/dashboard/Statusbar";
import MenuButtonsGroup from "../../components/dashboard/MenuButtonsGroup";
import { homeMenuSource } from "../../data/menu";
import MobileMenus from "../../components/dashboard/MobileMenus";
import Portal from "../../components/dashboard/Portal";
import New from "../../components/dashboard/New";
import { bookingColumns } from "../../data/PurchaseOrderData";
import { bookingFilterValues } from "../../helpers/datatableSource";
import webService from "../../axios/webService";
import ConfirmationPopupComponent from "../../components/dashboard/ConfirmationPopupComponent";
import OnboardingService from "../../axios/onboardingRequest";

const today = new Date().toISOString().slice(0, 10);

const Booking = () => {
  const [bookings, setBookings] = useState([]);
  const [singleBooking, setSingleBooking] = useState({});
  const [onRowDblClickBookingId, setRowDblClickBookingId] = useState(null);
  const [onRowClickItem, setRowClickItem] = useState(null);
  const [fromDate, setFromDate] = useState(today);
  const [toDate, setToDate] = useState(today);
  const [date, setDate] = useState("");
  const [statusMode, setStatusMode] = useState("");
  const [isOpen, setOpen] = useState(false);
  // eslint-disable-next-line
  const [loading, setLoading] = useState(false);

  const handleClose = () => {
    setRowDblClickBookingId(null);
    setSingleBooking({});
    setStatusMode("");
    setOpen(false);
  };

  const startEdit = ({ data }) => {
    if (data) {
      setRowDblClickBookingId(data.bookingId);
    } else {
      setRowDblClickBookingId(null);
    }
  };

  useEffect(() => {
    try {
      const getData = async () => {
        setLoading(true);
        // const response = date
        //   ? await webService.Request.getByDate(date.startdate, date.enddate)
        //   : await webService.Request.get();
        const url = "/test";
        const response = await OnboardingService.get(url);
        setLoading(false);
        setBookings(response);
      };
      getData();
    } catch (error) {
      console.log(error);
    }
  }, [date]);

  useEffect(() => {
    const getSingleBooking = async () => {
      const response = await webService.Request.getById(onRowDblClickBookingId);
      setSingleBooking(response);
      setStatusMode("EditMode");
      setOpen((isOpen) => !isOpen);
    };
    if (onRowDblClickBookingId) getSingleBooking();
  }, [onRowDblClickBookingId]);

  const openConfirmationPopup = async (rowItem) => {
    if (rowItem === null) {
      toast.warning("Please select a booking to delete");
    } else {
      setStatusMode("DeleteMode");
      setOpen((isOpen) => !isOpen);
    }
  };

  const handleClick = (menu) => {
    switch (menu) {
      case "Find":
        fromDate === null && toDate && date === ""
          ? setDate({ startdate: fromDate, enddate: toDate })
          : setDate({ startdate: fromDate, enddate: toDate });
        break;
      case "New":
        setStatusMode("CreateMode");
        setOpen((isOpen) => !isOpen);
        break;
      case "Delete":
        openConfirmationPopup(onRowClickItem);
        break;
      case "Close":
        console.log("Close was clicked");
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
            heading="Booking List"
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

                  <DateBox
                    id="fromDate"
                    onValueChanged={(e) => setFromDate(e.value)}
                    value={fromDate}
                    height={26}
                    style={{ fontSize: "12px" }}
                    className=" border pl-1 w-full md:w-1/2  outline-none"
                  />
                </div>
                <div className="flex w-full justify-between md:justify-start md:w-1/2 items-center gap-5">
                  <label
                    className="font-semibold text-xs  text-gray-600"
                    htmlFor="toDate"
                  >
                    To Date:
                  </label>
                  <DateBox
                    id="toDate"
                    onValueChanged={(e) => setToDate(e.value)}
                    value={toDate}
                    height={26}
                    style={{ fontSize: "12px" }}
                    className=" border pl-1 w-full md:w-1/2  outline-none"
                  />
                </div>
              </div>
            </article>
          </article>
        </section>
        <section className="mt-5">
          <DataTable
            data={bookings}
            columns={bookingColumns}
            keyExpr="bookingId"
            startEdit={(e) => startEdit(e)}
            //loading={loading}
            setRowClickItem={setRowClickItem}
            openConfirmationPopup={openConfirmationPopup}
            filterValues={bookingFilterValues}
          />
        </section>
      </section>

      {statusMode === "CreateMode" ? (
        <Portal isOpen={isOpen} setOpen={setOpen}>
          <New
            bookings={bookings}
            singleBooking={singleBooking}
            setBookings={setBookings}
            handleClose={handleClose}
            title={"Create New Booking"}
            heading={"Booking Item Management"}
            statusBarText={"New Booking Item"}
            statusMode={statusMode}
          />
        </Portal>
      ) : statusMode === "EditMode" ? (
        <Portal isOpen={isOpen} setOpen={setOpen}>
          <New
            bookings={bookings}
            singleBooking={singleBooking}
            setBookings={setBookings}
            handleClose={handleClose}
            title={"Update A Booking Item"}
            heading={"Booking Item Management"}
            statusBarText={"Updating Booking Item"}
            statusMode={statusMode}
          />
        </Portal>
      ) : (
        statusMode === "DeleteMode" && (
          <Portal isOpen={isOpen} setOpen={setOpen}>
            <ConfirmationPopupComponent
              item={onRowClickItem}
              bookings={bookings}
              setBookings={setBookings}
              handleClose={handleClose}
              title={"Delete A Booking Item"}
              statusBarText={"Delete Booking Item"}
              statusMode={statusMode}
            />
          </Portal>
        )
      )}

      <Statusbar heading="Booking List" company="ARBS Customer Portal" />
    </main>
  );
};

export default Booking;
