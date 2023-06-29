import pricings from "../../../data/pricing.json";
import PricingItem from "./pricing-item/PricingItem";

const Pricing = () => {
  return (
    <main className="py-3 px-8 md:py-8 md:px-20">
      <section>
        <article className="text-center">
          <h2 className="text-headingBlue text-xl md:text-4xl font-semibold">
            {pricings.header}
          </h2>
          <p className="w-full  m-5 mx-auto text-lg text-gray-600">
            {pricings.content}
          </p>
        </article>
        <article className="flex items-center flex-wrap justify-evenly">
          {pricings?.products?.map((item) => (
            <PricingItem key={item?.key} pricing={item} />
          ))}
        </article>
      </section>
    </main>
  );
};

export default Pricing;
