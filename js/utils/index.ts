/** Creates a promise which completes on a later turn of the message pump. Code which immediately
 * awaits the result of this function will definitely run asynchronously. */
export const forceAsync = async (arg?) => {
  await new Promise(resolve => setTimeout(resolve, 1));
  return arg;
};

/** Creates a promise which is already resolved. Code which awaits the results of this function will
 * immediately continue, synchronously, as the result is already available. */
export const synchronousAsync = (arg?) => Promise.resolve(arg);
