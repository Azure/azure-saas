import React from "react";
import "./supportform.css";
import { Address } from "../../contactus/Address";
import data from "../../../../data/pages/support";

export const SupportForm = () => {
  return (
    <div className="support-form">
      <div className="s-form">
        <p className="s-header">{data.header}</p>
        <p className="s-subheader">{data.subheader}</p>

        <div className="s-inputs">
          <div className="s-input-names">
            {data.inputs.names.map((input) => (
              <input
                type={input.type}
                placeholder={input.value}
                className="name-control"
              />
            ))}
          </div>
          {data.inputs.fields.map((input) => (
            <input
              type={input.type}
              placeholder={input.value}
              className="s-form-control"
            />
          ))}
          <textarea
            type="text"
            placeholder="Type your message here"
            className="s-textbox"
          />
          <button value="Submit" className="s-button">
            {data.btn_name}
          </button>
        </div>
      </div>
      <div className="s-form s-right">
        <Address />
      </div>
    </div>
  );
};
