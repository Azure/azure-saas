import { useEffect, useState } from "react";
import { TextBox } from "devextreme-react/text-box";
import SelectBox from "devextreme-react/select-box";
import Validator, { RequiredRule } from "devextreme-react/validator";
import { Button, NumberBox } from "devextreme-react";
import { FcAddDatabase } from "react-icons/fc";
import {
  industryOptions,
  onboardingQuestionsOptions,
  professionalOptions,
  servicePlanOptions,
} from "../../helpers/onBoardingSource";

import timezoneService from "../../helpers/timezones";
import {
  handleCategory,
  handleServicePlan,
} from "../../helpers/onBoardingFunction";
import { useNavigate } from "react-router-dom";
import services from "../../helpers/formDataSource";
import Portal from "../../components/dashboard/Portal";
import LoadingIndicator from "../../components/dashboard/LoadingIndicator";
import { useDispatch } from "react-redux";
import { getCurrentUser } from "../../services/userService";
import { getCRSFToken } from "../../helpers/auth";
import OnboardingService from "../../axios/onboardingRequest";
import Constant from "../../utils/constant";

const Onboarding = () => {
  const [isOpen, setOpen] = useState(false);
  const [loading, setLoading] = useState(false);

  const [industry, setIndustry] = useState(
    "Accounting, finance, banking, insuarance"
  );
  const [profession, setProfession] = useState(
    "Accounting, finance, banking, insuarance"
  );
  // eslint-disable-next-line
  const [categoryId, setCategoryId] = useState(null);

  const [employees, setEmployees] = useState(0);

  const [selectedTimezone, setSelectedTimezone] = useState(
    "E. Africa Standard Time"
  );
  const [tenantRouteName, setTenantRouteName] = useState("");
  const [timeZone, setTimezone] = useState("E. Africa Standard Time");
  const [onboardingQuestions, setOnboardingQuestions] = useState("");
  const [country, setCountry] = useState("Kenya");
  const [answer, setAnswer] = useState("");
  const [servicePlan, setServicePlan] = useState(
    "Standard (Ksh10,000, Free 14 Days Trial)"
  );
  // eslint-disable-next-line
  const [servicePlanNumber, setServicePlanNumber] = useState(null);

  const [organizationName, setOrganizationName] = useState("");

  const navigate = useNavigate();
  const dispatch = useDispatch();

  useEffect(() => {
    getCRSFToken();
  }, []);

  useEffect(() => {
    getCurrentUser(dispatch);
  }, [dispatch]);

  // Set the industry for the organisation according to what the user selects
  const handleIndustrySelection = (category) => {
    const selectedCategory = handleCategory(category);
    setIndustry(selectedCategory.name);
    setCategoryId(selectedCategory.key);
  };

  const handleServicePlanSelection = (servicePlan) => {
    const selectedService = handleServicePlan(servicePlan);
    setServicePlan(selectedService.name);
    setServicePlanNumber(selectedService.key);
  };

  const handleTimeZone = (selectedTimeZone) => {
    const allTimezones = timezoneService.getAllTimezones();
    allTimezones.filter((timezone) => {
      // Check the changes below
      if (timezone.text === selectedTimeZone) {
        setSelectedTimezone(timezone.text);
        setTimezone(timezone.value);
      }

      return 0;
    });
  };

  // Submit the tenant info to be processed and added to the database
  const onboardingFormData = {
    organizationName,
    categoryId,
    noofEmployees: employees,
    tenantRouteName,
    country,
    profession,
    timeZone,
    question: onboardingQuestions,
    answer,
    productTierId: servicePlanNumber,
  };

  const submitForm = async (e) => {
    e.preventDefault();
    setOpen(true);
    setLoading(true);

    let action = Constant.ACTION.ONBOARDING;

    try {
      const response = await OnboardingService.post(action, onboardingFormData);

      if (response) {
        setOpen(false);
        setLoading(false);
        navigate("/dashboard");
      }
    } catch (error) {
      setOpen(false);
      setLoading(false);
      console.log(error);
    }
  };

  return (
    <main className="min-h-screen ">
      <section className="p-3 md:py-5 md:px-0">
        <section className="w-full md:w-[50%] mx-auto">
          <article className="mb-5">
            <h1 className="font-bold text-2xl md:text-3xl mb-1 text-headingBlue">
              Welcome to the onboarding process
            </h1>
            <p className="font-semibold text-gray-700">
              Just a few steps to set you up
            </p>
          </article>
          <article>
            <form
              onSubmit={submitForm}
              className="flex flex-col md:items-start items-center"
            >
              <article className="flex flex-wrap">
                <div className="flex justify-between box-border flex-col gap-1  w-full mb-2">
                  <label
                    className="text-lg text-gray-800 font-medium"
                    htmlFor="organizationName"
                  >
                    What is your organization's name?
                  </label>
                  <span className="text-xs">
                    The name should be the name of your business, brand or
                    organization. You can change this later.
                  </span>
                  <TextBox
                    placeholder="Type here.."
                    onValueChanged={(e) => setOrganizationName(e.value)}
                    value={organizationName}
                    height={30}
                    style={{ fontSize: "12px" }}
                    className=" border pl-1 text-center w-full md:w-[70%] lg:w-[80%] outline-none"
                  >
                    <Validator>
                      <RequiredRule message="Organisation name is required" />
                    </Validator>
                  </TextBox>
                </div>
                <div className="flex justify-between box-border flex-col gap-1  w-full mb-2">
                  <label
                    className="text-lg text-gray-800 font-medium"
                    htmlFor="organizationCategory"
                  >
                    In what industry is your organization?
                  </label>
                  <span className="text-xs">
                    Identifying your industry will help people find you in
                    search results. Choose the closest one - you can update it
                    later.
                  </span>
                  <SelectBox
                    dataSource={industryOptions}
                    searchEnabled={true}
                    onValueChanged={(e) => handleIndustrySelection(e.value)}
                    value={industry}
                    placeholder="Select Organization Category"
                    height={30}
                    style={{ fontSize: "12px" }}
                    className="border pl-1 text-center w-full md:w-[70%] lg:w-[80%] outline-none"
                  />
                </div>
                <div className="flex justify-between box-border flex-col gap-1  w-full mb-2">
                  <label
                    className="text-lg text-gray-800 font-medium"
                    htmlFor="employees"
                  >
                    How many employees are in your organization?
                  </label>
                  <span className="text-xs">
                    The number of employees in your business, would determine
                    the tier to choose. You can change this later.
                  </span>
                  <NumberBox
                    id="employees"
                    onValueChanged={(e) => setEmployees(e.value)}
                    value={employees}
                    height={30}
                    style={{ fontSize: "12px" }}
                    className=" border pl-1 text-center w-full md:w-[70%] lg:w-[80%] outline-none"
                  >
                    {" "}
                    <Validator>
                      <RequiredRule message="Experience is required" />
                    </Validator>
                  </NumberBox>
                </div>

                <div className="flex justify-between box-border flex-col gap-1  w-full mb-2">
                  <label
                    className="text-lg text-gray-800 font-medium"
                    htmlFor="organizationCategory"
                  >
                    What is your profession?
                  </label>
                  <span className="text-xs">
                    Identifying your industry will help people find you in
                    search results. Choose the closest one - you can update it
                    later.
                  </span>
                  <SelectBox
                    dataSource={professionalOptions}
                    searchEnabled={true}
                    onValueChanged={(e) => setProfession(e.value)}
                    value={profession}
                    placeholder="Select Organization Category"
                    height={30}
                    style={{ fontSize: "12px" }}
                    className="border pl-1 text-center w-full md:w-[70%] lg:w-[80%] outline-none"
                  />
                </div>
                <div className="flex justify-between box-border flex-col gap-1  w-full mb-2">
                  <label
                    className="text-lg text-gray-800 font-medium"
                    htmlFor="tenantRouteName"
                  >
                    Please choose a name for your custom Tenant Route
                  </label>
                  <span className="text-xs">
                    Choose a unique route to use when accessing your tenant. You
                    can change this later.
                  </span>
                  <TextBox
                    placeholder="Type here.."
                    onValueChanged={(e) => setTenantRouteName(e.value)}
                    value={tenantRouteName}
                    height={30}
                    style={{ fontSize: "12px" }}
                    className=" border pl-1 text-center w-full md:w-[70%] lg:w-[80%] outline-none"
                  >
                    <Validator>
                      <RequiredRule message="Organisation name is required" />
                    </Validator>
                  </TextBox>
                </div>
                <div className="flex justify-between box-border flex-col gap-1  w-full mb-2">
                  <label
                    className="text-lg text-gray-800 font-medium"
                    htmlFor="question"
                  >
                    Please choose a country where your organisation is based.
                  </label>
                  <span className="text-xs">
                    Choose a country where your organisation is situated. You
                    can change this later.
                  </span>
                  <SelectBox
                    dataSource={countriesOptions}
                    searchEnabled={true}
                    onValueChanged={(e) => setCountry(e.value)}
                    value={country}
                    placeholder="Choose a security question"
                    height={30}
                    style={{ fontSize: "12px" }}
                    className="border pl-1 text-center w-full md:w-[70%] lg:w-[80%] outline-none"
                  />
                </div>
                <div className="flex justify-between box-border flex-col gap-1  w-full mb-2">
                  <label
                    className="text-lg text-gray-800 font-medium"
                    htmlFor="question"
                  >
                    Please choose question category
                  </label>
                  <span className="text-xs">
                    Choose any security question that is suitable for you. You
                    can change this later.
                  </span>
                  <SelectBox
                    dataSource={onboardingQuestionsOptions}
                    searchEnabled={true}
                    onValueChanged={(e) => setOnboardingQuestions(e.value)}
                    value={onboardingQuestions}
                    placeholder="Choose a security question"
                    height={30}
                    style={{ fontSize: "12px" }}
                    className="border pl-1 text-center w-full md:w-[70%] lg:w-[80%] outline-none"
                  />
                </div>
                <div className="flex justify-between box-border flex-col gap-1  w-full mb-2">
                  <label
                    className="text-lg text-gray-800 font-medium"
                    htmlFor="organizationName"
                  >
                    What is your answer to the question you have selected?
                  </label>
                  <span className="text-xs">
                    The answer should be familiar to you and would not be hard
                    to remember.
                  </span>
                  <TextBox
                    placeholder="Type here.."
                    onValueChanged={(e) => setAnswer(e.value)}
                    value={answer}
                    height={30}
                    style={{ fontSize: "12px" }}
                    className=" border pl-1 text-center w-full md:w-[70%] lg:w-[80%] outline-none"
                  >
                    <Validator>
                      <RequiredRule message="An answer is required" />
                    </Validator>
                  </TextBox>
                </div>
                <div className="flex justify-between box-border flex-col gap-1  w-full mb-2">
                  <label
                    className="text-lg text-gray-800 font-medium"
                    htmlFor="organizationCategory"
                  >
                    Choose your time zone
                  </label>
                  <span className="text-xs">
                    You can set your preffered timezone here.
                  </span>
                  <SelectBox
                    dataSource={timezonesOptions}
                    searchEnabled={true}
                    onValueChanged={(e) => handleTimeZone(e.value)}
                    value={selectedTimezone}
                    placeholder="Preffered timezone"
                    height={30}
                    style={{ fontSize: "12px" }}
                    className="border pl-1 text-center w-full md:w-[70%] lg:w-[80%] outline-none"
                  />
                </div>
                <div className="flex justify-between box-border flex-col gap-1  w-full mb-2">
                  <label
                    className="text-lg   text-gray-800 font-medium"
                    htmlFor="originCountry"
                  >
                    Select your subscription plan
                  </label>
                  <span className="text-xs">
                    Get to choose a preffered package here according to your
                    needs.
                  </span>
                  <SelectBox
                    dataSource={servicePlanOptions}
                    searchEnabled={true}
                    onValueChanged={(e) => handleServicePlanSelection(e.value)}
                    value={servicePlan}
                    placeholder="Select a Service Plan"
                    height={30}
                    style={{ fontSize: "12px" }}
                    className="border pl-1 text-center w-full md:w-[70%] lg:w-[80%] outline-none"
                  />
                </div>
              </article>
              <article>
                <Button id="onBoardingButton" useSubmitBehavior={true}>
                  {" "}
                  <FcAddDatabase className="text-white" fontSize={20} />
                  Submit
                </Button>
              </article>
            </form>
          </article>
        </section>
      </section>
      <Portal isOpen={isOpen} setOpen={setOpen}>
        <section className="bg-white w-full md:w-[600px]  mx-auto p-5 h-20 gap-2 rounded-sm flex flex-col items-center justify-center">
          <h2 className="text-lg">
            Please wait as we process your information.
          </h2>
          {loading && <LoadingIndicator />}
        </section>
      </Portal>
    </main>
  );
};

const timezonesOptions = timezoneService.getTimezoneText();
const countriesOptions = services.getCountries();

export default Onboarding;
