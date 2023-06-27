import { useCallback, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { homeMenuSource } from "../../../data/menu";
import Portal from "../../../components/dashboard/Portal";
import New from "../../../components/dashboard/New";
import { bookingColumns } from "../../../data/PurchaseOrderData";
import { bookingFilterValues } from "../../../helpers/datatableSource";
import ConfirmationPopupComponent from "../../../components/dashboard/ConfirmationPopupComponent";
import OnboardingService from "../../../axios/onboardingRequest";
import CategoryComponent from "../../../components/dashboard/CategoryComponent";
import Constant from "../../../utils/constant";
import FromToDateComponent from "../../../components/dashboard/FromToDateComponent";
import DataTable from "../../../components/dashboard/DataTable";
import Statusbar from "../../../components/dashboard/Statusbar";
import MenusGroupComponent from "../../../components/dashboard/Menus/MenusGroupComponent";

const Booking = () => {
  const [bookings, setBookings] = useState([]);
  const [singleBooking, setSingleBooking] = useState({});
  const [onEditRecordId, setEditRecordId] = useState(null);
  const [selectedRecordId, setSelectedRecordId] = useState(null);
  // eslint-disable-next-line
  const [date, setDate] = useState("");
  const [statusMode, setStatusMode] = useState("");
  const [isOpen, setOpen] = useState(false);

  const route = Constant.ROUTE.BOOKING;

  useEffect(() => {
    try {
      const getData = async () => {
        const url = "/test";
        const response = await OnboardingService.get(url);
        setBookings(response);
      };
      getData();
    } catch (error) {
      console.log(error);
    }

    return () => {};
  }, [date]);

  useEffect(() => {
    const getSingleRecord = async () => {
      const url = "/test/" + onEditRecordId;
      const response = await OnboardingService.get(url);
      setSingleBooking(response);
      setStatusMode("EditMode");
      setOpen((isOpen) => !isOpen);
    };
    if (onEditRecordId) getSingleRecord();
  }, [onEditRecordId]);

  const startEdit = useCallback(({ key }) => {
    if (key) {
      setEditRecordId(key);
    } else {
      setEditRecordId(null);
    }
  }, []);

  const selectRowItem = useCallback(({ key }) => {
    if (key) {
      setSelectedRecordId(key);
    }
  }, []);

  const handleClose = () => {
    setEditRecordId(null);
    setSingleBooking({});
    setStatusMode("");
    setOpen(false);
  };

  const openConfirmationPopup = useCallback(async (rowItem) => {
    if (rowItem === null) {
      toast.warning("Please select a booking to delete");
    } else {
      setStatusMode("DeleteMode");
      setOpen((isOpen) => !isOpen);
    }
  }, []);

  const handleDelete = async () => {
    console.log("delted");
  };

  const handleClick = useCallback(
    (menu) => {
      switch (menu) {
        case "Find":
          // fromDate === null && toDate && date === ""
          //   ? setDate({ startdate: fromDate, enddate: toDate })
          //   : setDate({ startdate: fromDate, enddate: toDate });
          break;
        case "New":
          setStatusMode("CreateMode");
          setOpen((isOpen) => !isOpen);
          break;
        case "Delete":
          openConfirmationPopup(selectedRecordId);
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
    },
    [selectedRecordId, openConfirmationPopup]
  );

  return (
    <main className="w-full min-h-full relative px-3 md:px-5 py-1.5">
      <section>
        <CategoryComponent>
          <MenusGroupComponent
            menus={homeMenuSource}
            heading={"Booking List"}
            onMenuClick={handleClick}
          />
          <FromToDateComponent />
          <DataTable
            data={bookings}
            route={route}
            keyExpr={"bookingId"}
            columns={bookingColumns}
            startEdit={startEdit}
            selectRowItem={selectRowItem}
            openConfirmationPopup={openConfirmationPopup}
            filterValues={bookingFilterValues}
          />
          <Statusbar
            heading={"Booking List"}
            company={"ARBS Customer Portal"}
          />
        </CategoryComponent>
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
            singleBooking={singleBooking}
            setSingleBooking={setSingleBooking}
            bookings={bookings}
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
              handleClose={handleClose}
              title={"Delete A Booking Item"}
              statusBarText={"Delete Booking Item"}
              statusMode={statusMode}
              onDelete={handleDelete}
            />
          </Portal>
        )
      )}
      {/* <div>
      <a data-refid="recordId" data-special-link="true" title="Maurice Nganga" data-navigable="true" target="_blank" data-ownerid="2175:0" data-recordid="0018d00000eyAglAAE" rel="noreferrer" href="/lightning/r/0018d00000eyAglAAE/view" class="slds-truncate outputLookupLink slds-truncate outputLookupLink-0018d00000eyAglAAE-2175:0 forceOutputLookup" data-aura-rendered-by="2186:0" data-aura-class="forceOutputLookup">Maurice Nganga<!--render facet: 2189:0--></a>
      
      </div> */}
    </main>
  );
};

export default Booking;
