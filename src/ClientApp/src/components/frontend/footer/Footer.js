import React from 'react';
import './footer.css';
import { AboutSec } from './components/AboutSec';
import { Navigation } from './components/Navigation';

export const Footer = () => {
  return (
      <div className='footer'>
        <AboutSec />
        <Navigation />
      </div>  
  )
}
