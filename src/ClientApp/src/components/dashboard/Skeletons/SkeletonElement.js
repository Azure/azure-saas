import "./Skeleton.css";
const SkeletonElement = ({ type }) => {
  const classes = `skeleton ${type}`;
  return <main className={classes}></main>;
};

export default SkeletonElement;
