import React from 'react';
import { TextInput, TextInputProps } from 'react-native-paper';
import { StyleSheet } from 'react-native';

const StyledTextInput = (props: TextInputProps) => {
  return (
    <TextInput
      mode="outlined"
      {...props}
      style={[styles.input, props.style]}
    />
  );
};

const styles = StyleSheet.create({
  input: {
    marginBottom: 12,
  },
});

export default StyledTextInput;