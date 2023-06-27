import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import { updateUserProfile } from "../../../redux/userSlice";
import { updateMenuSource } from "../../../data/menu";
import Statusbar from "../../../components/dashboard/Statusbar";
import axios from "axios";
import MenusGroupComponent from "../../../components/dashboard/Menus/MenusGroupComponent";

const Profile = () => {
  const currentUser = useSelector((state) => state.user?.currentUser?.user);
  const [formInputs, setFormInputs] = useState(currentUser);

  const dispatch = useDispatch();
  const navigate = useNavigate();

  const handleChange = (event) => {
    const { name, value } = event.target;
    setFormInputs({ ...formInputs, [name]: value });
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    try {
      const { data } = await axios.put("/User", formInputs);

      dispatch(updateUserProfile(data));
    } catch (error) {
      console.log(error);
    }
  };

  const handleClick = (menu) => {
    switch (menu) {
      case "Update":
        handleSubmit();
        break;
      case "Close":
        navigate("/");
        break;
      default:
        break;
    }
  };

  return (
    <main className="w-full relative h-full md:h-full  px-3 md:px-5 py-1.5">
      <section>
        <MenusGroupComponent
          heading="Update Profile"
          menus={updateMenuSource}
          onMenuClick={handleClick}
        />

        <section>
          <article className="md:p-5 flex gap-5 w-full">
            <article className="flex flex-col md:flex-row w-full md:w-1/2 p-5 relative shadow-none md:shadow-xl">
              <h2 className="mb-5 text-menu">Information</h2>
              <div className="flex flex-col md:flex-row gap-5">
                <img
                  className="w-[6.25rem] h-[6.25rem] self-center md:self-start rounded-full object-cover cursor-pointer"
                  src="https://upload.wikimedia.org/wikipedia/commons/thumb/6/65/No-Image-Placeholder.svg/1665px-No-Image-Placeholder.svg.png"
                  alt="profile"
                />
                <div>
                  <h2 className="mb-2.5 text-menu font-medium">User Profile</h2>
                  <div className="mb-2.5 text-[14px]">
                    <span className=" font-bold mr-1 text-menu">Full Name</span>
                    <span className="text-text font-medium">
                      {currentUser?.fullName}
                    </span>
                  </div>
                  <div className="mb-2.5 text-[14px]">
                    <span className=" font-bold mr-1 text-menu">Username</span>
                    <span className="text-text font-medium">
                      {currentUser?.userName}
                    </span>
                  </div>
                  <div className="mb-2.5 text-[14px]">
                    <span className=" font-bold mr-1 text-menu">Email</span>
                    <span className="text-text font-medium">
                      {currentUser?.email}
                    </span>
                  </div>
                  <div className="mb-2.5 text-[14px]">
                    <span className=" font-bold mr-1 text-menu">Telephone</span>
                    <span className="text-text font-medium">
                      {currentUser?.telephone}
                    </span>
                  </div>
                  <div className="mb-2.5 text-[14px]">
                    <span className=" font-bold mr-1 text-menu">
                      Physical Address
                    </span>
                    <span className="text-text font-medium">
                      {currentUser?.physicalAddress}
                    </span>
                  </div>
                  <div className="mb-2.5 text-[14px]">
                    <span className=" font-bold mr-1 text-menu">
                      Origin Country
                    </span>
                    <span className="text-text font-medium">
                      {currentUser?.originCountry}
                    </span>
                  </div>
                </div>
              </div>
            </article>
            <article className="hidden md:block w-1/2"></article>
          </article>
        </section>
      </section>
      {/* Bottom Section */}
      <section>
        <div className="w-full p-2 md:p-5">
          <div className="text-menu bg-bgxLight rounded-t-md py-1 px-2 font-medium ">
            Enter all the details in the fields below then click save.
          </div>
          <form className="flex w-full mt-1 py-4 md:py-3 rounded-sm flex-wrap justify-between gap-2">
            <div className="flex justify-between flex-col gap-3 md:gap-0 md:flex-row w-full md:w-[45%]">
              <label
                className="text-sm font-medium text-gray-600"
                htmlFor="fullName"
              >
                Full Name:<sup className=" text-red-600">*</sup>
              </label>
              <input
                className="border border-menu rounded-sm pl-1 w-full md:w-1/2 outline-none "
                type="text"
                id="fullName"
                name="fullName"
                onChange={handleChange}
                value={formInputs.fullName}
              />
            </div>
            <div className="flex justify-between flex-col gap-3 md:gap-0 md:flex-row w-full md:w-[45%]">
              <label
                className="text-sm font-medium text-gray-600"
                htmlFor="userName"
              >
                Username:<sup className=" text-red-600">*</sup>
              </label>
              <input
                className="border border-menu rounded-sm pl-1 w-full md:w-1/2 outline-none "
                type="text"
                id="userName"
                name="userName"
                onChange={handleChange}
                value={formInputs.userName}
              />
            </div>
            <div className="flex justify-between flex-col gap-3 md:gap-0 md:flex-row w-full md:w-[45%]">
              <label
                className="text-sm font-medium text-gray-600"
                htmlFor="email"
              >
                Email:<sup className=" text-red-600">*</sup>
              </label>
              <input
                className="border border-menu rounded-sm pl-1 w-full md:w-1/2 outline-none "
                type="email"
                id="email"
                name="email"
                onChange={handleChange}
                value={formInputs.email}
              />
            </div>
            <div className="flex justify-between flex-col gap-3 md:gap-0 md:flex-row w-full md:w-[45%]">
              <label
                className="text-sm font-medium text-gray-600"
                htmlFor="telephone"
              >
                Telephone:<sup className=" text-red-600">*</sup>
              </label>
              <input
                className="border border-menu rounded-sm pl-1 w-full md:w-1/2 outline-none "
                type="text"
                id="telephone"
                name="telephone"
                onChange={handleChange}
                value={formInputs.telephone}
              />
            </div>
            <div className="flex justify-between flex-col gap-3 md:gap-0 md:flex-row w-full md:w-[45%]">
              <label
                className="text-sm font-medium text-gray-600"
                htmlFor="physicalAddress"
              >
                Physical Address:<sup className=" text-red-600">*</sup>
              </label>
              <input
                className="border border-menu rounded-sm pl-1 w-full md:w-1/2 outline-none "
                type="text"
                id="physicalAddress"
                name="physicalAddress"
                onChange={handleChange}
                value={formInputs.physicalAddress}
              />
            </div>

            <div className="flex justify-between flex-col gap-3 md:gap-0 md:flex-row w-full md:w-[45%]">
              <label
                className="text-sm font-medium text-gray-600"
                htmlFor="originCountry"
              >
                Origin Country:<sup className=" text-red-600">*</sup>
              </label>
              <input
                className="border border-menu rounded-sm pl-1 w-full md:w-1/2 outline-none "
                type="text"
                id="originCountry"
                name="originCountry"
                onChange={handleChange}
                value={formInputs.originCountry}
              />
            </div>

            <div className="flex justify-between flex-col gap-3 md:gap-0 md:flex-row w-full md:w-[45%]">
              <label
                className="text-sm font-medium text-gray-600"
                htmlFor="experience"
              >
                Experience (years):<sup className=" text-red-600">*</sup>
              </label>
              <input
                className="border border-menu rounded-sm pl-1 w-full md:w-1/2 outline-none "
                type="number"
                id="experience"
                name="experience"
                onChange={handleChange}
                value={formInputs.experience}
              />
            </div>
            <div className="flex justify-between flex-col gap-3 md:gap-0 md:flex-row w-full md:w-[45%]">
              <label
                className="text-sm font-medium text-gray-600"
                htmlFor="position"
              >
                Position:<sup className=" text-red-600">*</sup>
              </label>
              <input
                className="border border-menu rounded-sm pl-1 w-full md:w-1/2 outline-none "
                type="text"
                id="position"
                name="position"
                onChange={handleChange}
                value={formInputs.position}
              />
            </div>
            <div className="flex justify-between flex-col gap-3 md:gap-0 md:flex-row w-full md:w-[45%]">
              <label
                className="text-sm font-medium text-gray-600"
                htmlFor="disabilityStatus"
              >
                Disability Status:<sup className=" text-red-600">*</sup>
              </label>
              <select
                className="w-full md:w-1/2  border border-menu rounded-sm pl-1 outline-none placeholder:text-sm outline-blue text-menu"
                id="disabilityStatus"
                name="disabilityStatus"
                onChange={handleChange}
                defaultValue={currentUser.disabilityStatus}
              >
                <option className=" rounded-none p-5 text-sm" value="Disabled">
                  Disabled
                </option>
                <option
                  className=" rounded-none p-5 text-sm"
                  value="Not Disabled"
                >
                  Not Disabled
                </option>
              </select>
            </div>
          </form>
        </div>
      </section>
      <Statusbar
        heading={`Welcome ${currentUser?.fullName}`}
        company="ARBS Customer Portal"
      />
    </main>
  );
};

export default Profile;
