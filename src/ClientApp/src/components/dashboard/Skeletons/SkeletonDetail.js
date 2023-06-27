import SkeletonElement from "./SkeletonElement";
import "./Skeleton.css";
import Shimmer from "./Shimmer";

const SkeletonDetail = () => {
  return (
    <main className="skeleton-wrapper">
      <section className="skeleton-detail">
        <article className="skeleton-wrapper card">
          <SkeletonElement type="title" />
          <SkeletonElement type="text" />
          <Shimmer />
        </article>
        <article className="skeleton-wrapper card">
          <SkeletonElement type="title" />
          <SkeletonElement type="text" />
          <Shimmer />
        </article>
      </section>
    </main>
  );
};

export default SkeletonDetail;
