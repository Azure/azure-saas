// Memoizing message div to avoid unnecessary rerender
export const MessageDiv = ({ message }) => {
  return <p className="po-message" style={{color: 'grey', fontSize: '.8rem'}}>{message}</p>;
};

// End
