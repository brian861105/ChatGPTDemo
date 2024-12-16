import * as yup from 'yup';

const emailRule = yup.string().required().email('Must be valid email');

const pwdRule = yup.string().required('Password is required').min(8);

const pwdConfirmRule = yup.string().required('Please Enter the password again').oneOf([yup.ref('pwd')],'Password must match');

const nameRule = yup.string().required('Reguired field');


export  {emailRule, pwdRule, pwdConfirmRule, nameRule};