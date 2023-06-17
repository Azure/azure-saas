export const handleCategory = (category) => {
  switch (category) {
    case "Automotive, Mobility & Transportation":
      return { name: category, key: 1 };

    case "Energy & Sustainability":
      return { name: category, key: 2 };
    case "Finance Services":
      return { name: category, key: 3 };

    case "Healthcare & Life Sciences":
      return { name: category, key: 4 };

    case "Manufacturing & Supply Chain":
      return { name: category, key: 5 };

    case "Media & Communications":
      return { name: category, key: 6 };

    case "Public Sector":
      return { name: category, key: 7 };

    case "Retail & Consumer Goods":
      return { name: category, key: 8 };

    case "Software":
      return { name: category, key: 9 };

    default:
      break;
  }
};

export const handleServicePlan = (servicePlan) => {
  switch (servicePlan) {
    case "Free (Free Forever)":
      return { name: servicePlan, key: 5 };

    case "Basic (Ksh3,000, Free 7 Days Trial)":
      return { name: servicePlan, key: 6 };

    case "Standard (Ksh10,000, Free 14 Days Trial)":
      return { name: servicePlan, key: 7 };

    default:
      return;
  }
};
