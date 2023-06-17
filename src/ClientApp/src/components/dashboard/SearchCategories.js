import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { categories } from "../../helpers/categoriesSource";

const SearchCategories = ({ searchInput, setSearchInput }) => {
  const [searchResult, setSearchResult] = useState([]);
  const [isExpanded, setIsExpanded] = useState(false);

  useEffect(() => {
    if (searchInput === "") {
      setSearchResult(categories);
      setIsExpanded(false);
    } else {
      setIsExpanded(true);
      const resultArray = searchResult.filter((result) =>
        result.title.toLowerCase().includes(searchInput)
      );
      setSearchResult(resultArray);
    }
  }, 
  // eslint-disable-next-line
  [searchInput]);

  const handleExpand = () => {
    setSearchInput("");
    setIsExpanded(false);
  };

  return (
    <>
      <div
        className={`absolute ${
          searchInput === "" && !isExpanded ? "hidden" : "block"
        } search z-50 w-[90%] md:w-[380px] bg-white top-9  shadow-xl`}
      >
        <ul className="text-left cursor-pointer group">
          {searchResult.map((category) => (
            <li
              key={category?.id}
              className="text-text text-sm py-1.5 px-5 hover:bg-bgxLight"
              onClick={handleExpand}
            >
              <Link to={`${category.link}`}>{category.title}</Link>
            </li>
          ))}
        </ul>
      </div>
    </>
  );
};

export default SearchCategories;
