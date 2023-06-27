import React from 'react'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faFacebook, faYoutube, faTwitter, faLinkedin  } from '@fortawesome/free-brands-svg-icons';

export const Socials = () => {
  return (
      <div className='socials-icons'>
          <a href="https://www.facebook.com/profile.php?id=100092993799371"><FontAwesomeIcon icon={faFacebook} className='social-icon' /></a>
          <a href="https://twitter.com/BusinessCl55620"><FontAwesomeIcon icon={faTwitter} className='social-icon' /></a>
          <FontAwesomeIcon icon={faYoutube} className='social-icon' />
          <a href="https://www.linkedin.com/company/businesscloud-limited/?viewAsMember=true"><FontAwesomeIcon icon={faLinkedin} className='social-icon' /></a>
      </div>
  )
}
