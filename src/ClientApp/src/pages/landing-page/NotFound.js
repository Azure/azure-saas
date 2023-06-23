import React from 'react'
import { Copyright } from "../../components/frontend/copyright/Copyright";
import notfound from "../../data/notfound"

export const NotFound = () => {
  return (
    <>
      <div style={{
          height: '37.7vw',
          display: 'flex',
          justifyContent: 'center',
          flexDirection: 'column',
          alignItems: 'center'

      }}>
          <p className='notfound-header'>{notfound.code}</p>
          <p className='notfound-text'>{notfound.text}</p>
      
      </div>
      <Copyright />
    </>
  )
}
