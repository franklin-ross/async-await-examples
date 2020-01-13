/** Creates a promise which completes on a later turn of the message pump. Code which immediately
 * awaits the result of this function will definitely run asynchronously. */
export const forceAsync = () => new Promise(resolve => setTimeout(resolve, 1));

/** Creates a promise which is already resolved. Code which awaits the results of this function will
 * immediately continue, synchronously, as the result is already available. */
export const synchronousAsync = () =>
  new Promise(resolve => setTimeout(resolve, 1));
