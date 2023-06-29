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
import ianasqltimezones from "../../data/ianasqltimezones";
import onboardingText from "../../data/onboarding"

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
  const [timeZone, setTimezone] = useState("E. Africa Standard Time");
  const [onboardingQuestions, setOnboardingQuestions] = useState("");
  const [country, setCountry] = useState("Kenya");
  const [answer, setAnswer] = useState("");
  const [servicePlan, setServicePlan] = useState(
    "Standard (Ksh10,000, Free 14 Days Trial)"
  );
  // eslint-disable-next-line
  const [servicePlanNumber, setServicePlanNumber] = useState(7);

  const [organizationName, setOrganizationName] = useState("");
  const navigate = useNavigate();
  const dispatch = useDispatch();

  useEffect(() => {
    getCRSFToken();
    getCurrentTimezone();
  }, []);

  useEffect(() => {
    getCurrentUser(dispatch);
  }, [dispatch]);

  const getCurrentTimezone = () => {
    const timeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;
    const currentZone = ianasqltimezones.filter((a) => {
      return a.ianaTimezone === timeZone;
    });
    const sqlZone = timezonesComplete.filter(
      (tz) => tz.value === currentZone[0].sqlTimeZone
    );
    return setSelectedTimezone(sqlZone[0].text);
  };

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
      if (timezone.text === selectedTimeZone) {
        setSelectedTimezone(timezone.text);
        setTimezone(timezone.value);
      }

      return 0;
    });
  };

  const onboardingFormData = {
    organizationName,
    categoryId,
    noofEmployees: employees,
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
              {onboardingText.header.title}
            </h1>
            <p className="font-semibold text-gray-700">
              {onboardingText.header.subtitle}
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
                    {onboardingText.fields.organizationName.title}
                  </label>
                  <span className="text-xs">
                    {onboardingText.fields.organizationName.subtitle}
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
                    {onboardingText.fields.industry.title}
                  </label>
                  <span className="text-xs">
                    {onboardingText.fields.industry.subtitle}
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
                    {onboardingText.fields.employeeNumber.title}
                  </label>
                  <span className="text-xs">
                    {onboardingText.fields.employeeNumber.subtitle}
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
                    {onboardingText.fields.jobTitle.title}
                  </label>
                  <span className="text-xs">
                    {onboardingText.fields.jobTitle.subtitle}
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
                    htmlFor="question"
                  >
                    {onboardingText.fields.countryBased.title}
                  </label>
                  <span className="text-xs">
                    {onboardingText.fields.countryBased.subtitle}
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
                    {onboardingText.fields.verificationQn.title}
                  </label>
                  <span className="text-xs">
                    {onboardingText.fields.verificationQn.subtitle}
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
                    {onboardingText.fields.verificationAns.title}
                  </label>
                  <span className="text-xs">
                    {onboardingText.fields.verificationAns.subtitle}
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
                    {onboardingText.fields.timezoneSelector.title}
                  </label>
                  <span className="text-xs">
                    {onboardingText.fields.timezoneSelector.subtitle}
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
                    {onboardingText.fields.subscriptionSelector.title}
                  </label>
                  <span className="text-xs">
                    {onboardingText.fields.subscriptionSelector.subtitle}
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
                  {onboardingText.fields.submitBtn.text}
                </Button>
              </article>
            </form>
          </article>
        </section>
      </section>
      <Portal isOpen={isOpen} setOpen={setOpen}>
        <section className="bg-white w-full md:w-[600px]  mx-auto p-5 h-20 gap-2 rounded-sm flex flex-col items-center justify-center">
          <h2 className="text-lg">
            {onboardingText.fields.onSubmitting.text}
          </h2>
          {loading && <LoadingIndicator />}
        </section>
      </Portal>
    </main>
  );
};

const timezonesOptions = timezoneService.getTimezoneText();
const timezonesComplete = timezoneService.getAllTimezones();
const countriesOptions = services.getCountries();

export default Onboarding;
