import React from 'react';
import { Button, ButtonProps } from 'react-native-paper';
import { StyleSheet } from 'react-native';

const StyledButton = (props: ButtonProps) => {
  return (
    <Button
      {...props}
      style={[styles.button, props.style]}
      labelStyle={[styles.label, props.labelStyle]}
    />
  );
};

const styles = StyleSheet.create({
  button: {
    marginTop: 12,
    paddingVertical: 6,
    borderRadius: 8,
  },
  label: {
    fontWeight: 'bold',
  },
});

export default StyledButton;